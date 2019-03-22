// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.



using System.Linq;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;

namespace InfoVis.MixedReality.Interaction.Handlers
{

    public class DataBallGazeHandler : BaseFocusHandler
    {
        public GameObject infoPanel;

        private Coroutine delayFocus;

        private IEnumerator DelayFocusExit(float time)
        {
            yield return time > 0 ? new WaitForSeconds(time) : null;
            infoPanel.SetActive(false);
            delayFocus = null;
        }

        public override void OnFocusEnter(FocusEventData focusEvent)
        {
            base.OnFocusEnter(focusEvent);
            infoPanel.SetActive(true);
        }

        public override void OnFocusExit(FocusEventData focusEvent)
        {
            base.OnFocusExit(focusEvent);

            if (delayFocus != null)
            {
                StopCoroutine(delayFocus);
            }

            delayFocus = StartCoroutine(DelayFocusExit(0f));
        }
    }
}
