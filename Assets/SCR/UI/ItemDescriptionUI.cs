using TMPro;
using UnityEngine;

public class ItemDescriptionUI : MonoBehaviour
{
    public static ItemDescriptionUI Instance;
    [SerializeField] private GameObject panelText;
    [SerializeField] private TMP_Text descriptionText;
    private bool isShowing = false;
    public float timeToDisable = 3f;
    private float tmpTimeToDisable;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        tmpTimeToDisable = timeToDisable;
        if (panelText != null)
            panelText.SetActive(false);
    }

    private void Update()
    {
        if (!isShowing) return;
        tmpTimeToDisable -= Time.deltaTime;
        if (tmpTimeToDisable <= 0f)
            Hide();
    }

    public void Show(ItemData item)
    {
        if (item == null)
        {
            Hide();
            return;
        }
        if (panelText == null || descriptionText == null) return;

        isShowing = true;
        tmpTimeToDisable = timeToDisable;
        panelText.SetActive(true);

        string finalText = item.description;

        if (item.unlockRecipes != null && item.unlockRecipes.Length > 0)
        {
            foreach (var recipe in item.unlockRecipes)
            {
                if (recipe != null && !DialogueFlags.HasRecipe(recipe.recipeId))
                {
                    DialogueFlags.UnlockRecipe(recipe.recipeId);
                    finalText += "\n\n<color=#55FF55><b>Вы изучили новый рецепт!</b></color>";
                }
            }
        }

        descriptionText.text = finalText;
    }
    private void Hide()
    {
        isShowing = false;

        if (panelText != null)
            panelText.SetActive(false);
    }
}

