using System.Collections;
using System.Linq;
using UnityEngine;

using InfoVis.MixedReality.Serializations;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.SDK.Utilities.Solvers;
using TMPro;

namespace InfoVis.MixedReality.Actions
{
    public class ObjectSpawner
    {
        GameObject origin = GameObject.FindGameObjectWithTag("ObjectSpawner/Mesh");
        float initialSize;

        public ObjectSpawner()
        {
            initialSize = origin.GetComponent<Renderer>().bounds.size.x;
        }

        public GameObject Spawn(MonoBehaviour Host, string name, float destroyAfter = 10f)
        {
            GameObject item = GameObject.Instantiate(Resources.Load<GameObject>(name));
            item.transform.position = origin.transform.position;

            Host.StartCoroutine(TimedActions.DestroyTimer(destroyAfter, item));
            Host.StartCoroutine(TimedActions.FadeTo(destroyAfter, 0f, item));

            return item;
        }

        public float getScale()
        {
            return origin.GetComponent<Renderer>().bounds.size.x / initialSize;
        }
    }

    class TimedActions
    {
        /// <summary>
        /// Destroys a GameObject after a specified amount of time
        /// </summary>
        /// <param name="time"></param>
        /// <param name="toDestroy"></param>
        /// <returns></returns>
        public static IEnumerator DestroyTimer(float time, GameObject toDestroy)
        {
            yield return new WaitForSeconds(time);
            GameObject.Destroy(toDestroy);
        }

        /// <summary>
        /// Linearly decreases the alpha channel for all materials in a given GameObject over a specified duration
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="targetAlpha"></param>
        /// <param name="toFade"></param>
        /// <returns></returns>
        public static IEnumerator FadeTo(float duration, float targetAlpha, GameObject toFade)
        {
            MeshRenderer renderer = toFade.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                System.Tuple<Material, float>[] materials = renderer.materials.Select(
                    m => new System.Tuple<Material, float>(m, m.color.a)
                ).ToArray();

                for (float t = 0.0f; t < duration; t += Time.deltaTime)
                {
                    foreach (var tuple in materials)
                    {
                        Material m = tuple.Item1;
                        Color curr = m.color;
                        curr.a = Mathf.Lerp(tuple.Item2, targetAlpha, t / duration);
                        m.color = curr;
                    }

                    yield return null;
                }
            }
        }

        public static IEnumerator SpawnSeries(MonoBehaviour Host, ObjectSpawner spawner, float destroyAfter, TimeSeriesData datasource, float tBetweeenItems, string visualizationName)
        {

            Manipulation.UpdatePanelVisName(visualizationName);

            int max = datasource.datasets.Select(d => d.data.Max()).Max();
            int min = datasource.datasets.Select(d => d.data.Min()).Min();


            for (int i = 0; i < datasource.labels.Length; ++i)
            {

                Manipulation.UpdatePanelInfo($"{datasource.labels[i]}");

                foreach (TimeSeriesData.Dataset dataset in datasource.datasets)
                {                
                    string objName = "";

                    switch (dataset.label) {
                        case "PLAYERUNKNOWN'S BATTLEGROUNDS": objName = "DataBall-PUBG";     break;
                        case "Apex Legends":                  objName = "DataBall-Apex";     break;
                        case "Fortnite":                      objName = "DataBall-Fortnite"; break;

                        default:
                            break;
                    }

                    int datapoint = dataset.data[i];

                    float spawnerScale = spawner.getScale();
                    float size = Mathf.InverseLerp(min, max, datapoint) * spawnerScale;

                    if (size > 0)
                    {
                        GameObject item = spawner.Spawn(Host, objName, destroyAfter);
                        Manipulation.SizeAndLabelDataball(size, $"{ datapoint }", item);
                    }
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(tBetweeenItems);
            }

            Manipulation.UpdatePanelVisName("No visualization running");
        }
    }

    public class Manipulation
    {
        public static GameObject SizeAndLabelDataball(float scale, string label, GameObject dataBall)
        {
            if (dataBall != null)
            {
                GameObject sphere = dataBall.transform.Find("Sphere").gameObject;
                GameObject infoPanel = dataBall.transform.Find("InfoPanel").gameObject;
                GameObject valueText = dataBall.transform.Find("InfoPanel/Canvas/DataValue").gameObject;

                if (sphere != null)
                {
                    sphere.transform.localScale = new Vector3(scale, scale, scale);

                    if (infoPanel != null)
                    {
                        infoPanel.GetComponent<Orbital>().WorldOffset = new Vector3(0, sphere.GetComponent<Renderer>().bounds.size.y + 0.1f, 0);

                        if (valueText != null)
                        {
                            TextMeshProUGUI textMesh = valueText.GetComponent<TextMeshProUGUI>();
                            if (textMesh != null)
                            {
                                textMesh.text = label;
                            }
                        }
                    }
                }
            }

            return dataBall;
        }

        public static void UpdatePanelInfo(string infoText)
        {
            TextMesh infoTextMesh = GameObject.FindGameObjectWithTag("InfoPanel/InfoText").GetComponent<TextMesh>();
            infoTextMesh.text = infoText;
        }


        public static void UpdatePanelVisName(string visName)
        {
            TextMesh visNameMesh = GameObject.FindGameObjectWithTag("InfoPanel/VisName").GetComponent<TextMesh>();
            visNameMesh.text = visName;
        }
    }
}
