using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Header("Ссылки")]
    [Tooltip("Кнопки")]
    [SerializeField] private GameObject CanselButton;

    [Tooltip("Включить/Выключить")]
    [SerializeField] private GameObject Slider;
    [SerializeField] private GameObject ObjOne;
    [SerializeField] private GameObject ObjTwo;
    [SerializeField] private GameObject ObjThree;

    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void Cancel()
    {
        Slider.SetActive(false);
        CanselButton.SetActive(false);
        ObjOne.SetActive(true);
        ObjTwo.SetActive(true);
        ObjThree.SetActive(true);
    }
}
