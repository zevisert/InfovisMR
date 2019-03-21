using System.Collections;
using System.Linq;
using UnityEngine;

using InfoVis.MixedReality.Serializations;
using System.Collections.Generic;

namespace InfoVis.MixedReality.Actions
{
    public class ObjectSpawner
    {
        GameObject origin = GameObject.FindGameObjectWithTag("ObjectSpawner");

        public ObjectSpawner()
        { }

        public GameObject Spawn(MonoBehaviour Host, string name)
        {
            GameObject item = GameObject.Instantiate(Resources.Load<GameObject>(name));
            item.transform.position = origin.transform.position;

            Host.StartCoroutine(TimedActions.DestroyTimer(10f, item));
            Host.StartCoroutine(TimedActions.FadeTo(10f, 0f, item));

            return item;
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

        public static IEnumerator SpawnSeries(System.Func<GameObject> spawn, TimeSeriesData.Dataset datasource, float tBetweeenItems)
        {
            int max = datasource.data.Max();
            int min = datasource.data.Min();

            foreach (var datapoint in datasource.data)
            {
                GameObject item = spawn();

                float size = Mathf.InverseLerp(min, max, datapoint);
                item.transform.localScale += new Vector3(size, size, size);

                yield return new WaitForSeconds(tBetweeenItems);
            }
        }
    }
}
