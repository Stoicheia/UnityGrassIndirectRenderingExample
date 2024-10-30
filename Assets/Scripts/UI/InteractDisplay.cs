using System;
using MagicGrass.StarStrings;
using TMPro;
using UnityEngine;

namespace MagicGrass.UI
{
    public class InteractDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _toInteractField;
        [SerializeField] private Interactor _interactor;

        private void Update()
        {
            _toInteractField.text = _interactor.InRange.PromptText;
        }
    }
}