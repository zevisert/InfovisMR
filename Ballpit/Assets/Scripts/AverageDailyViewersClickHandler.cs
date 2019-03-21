
using System.IO;
using System.Linq;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Events;
using Microsoft.MixedReality.Toolkit.SDK.UX.Interactable;
using Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.States;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;

using InfoVis.MixedReality.Serializations;
using InfoVis.MixedReality.Actions;


namespace InfoVis.MixedReality.Interaction.Handlers
{

    public class AverageDailyViewersClickHandler : ReceiverBase
    {
        private string visDataFileName = "average_daily_viewers.json";
        private TimeSeriesData data;

        protected ObjectSpawner Spawner { get; } = new ObjectSpawner();
        protected Coroutine spawnCoro = null;


        public AverageDailyViewersClickHandler(UnityEvent ev) : base(ev)
        {
            Name = "AverageDailyViewersEvent";
            HideUnityEvents = true;

            string filePath = Path.Combine(Application.streamingAssetsPath, visDataFileName);

            if (File.Exists(filePath))
            {
                string dataAsJsonText = File.ReadAllText(filePath);
                data = JsonUtility.FromJson<TimeSeriesData>(dataAsJsonText);

                Debug.Log($"Loaded data for {data.datasets.Select(d => d.label).Aggregate("", (agg, name) => $"{agg}, {name}")}");
            }
        }

        private IEnumerator SpawnDatasets()
        {
            TimeSeriesData.Dataset d = data.datasets[0];
            
            yield return TimedActions.SpawnSeries(() => Spawner.Spawn(Host, "Icosa"), d, 0.1f);

            // Set button state back to Default
            Debug.Log("Done spawning");           
            spawnCoro = null;
        }

        public override void OnUpdate(InteractableStates state, Interactable source)
        {
     
        }

        public override void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = null)
        {
            base.OnClick(state, source, pointer);

            Debug.Log($"Clicked ${data.datasets[0].label}");
            
            if (spawnCoro == null)
            {
                spawnCoro = Host.StartCoroutine(SpawnDatasets());
            }
            else
            {
                Host.StopCoroutine(spawnCoro);
            }
        }

        public override void OnVoiceCommand(InteractableStates state, Interactable source, string command, int index = 0, int length = 1)
        {
            base.OnVoiceCommand(state, source, command, index, length);
        }


    }
}
