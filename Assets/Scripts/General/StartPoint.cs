using UnityEngine;

namespace General
{
    public class StartPoint : MonoBehaviour
    {
        public GameObject HintPlace;
        // Update is called once per frame
        void Update()
        {
            HintPlace.SetActive(Clickable.HasUnitSelected);
        }
    }
}