using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropMe : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Text textObject;

    public Image containerImage;
    public Image receivingImage;
    private Color normalColor;
    public Color highlightColor = Color.yellow;

    private string myPositionName;

    // 他のフィールドやメソッドは省略...

    private void Start()
    {
        if (textObject != null)
        {
            Debug.Log("textObject " + textObject.text);
            myPositionName = textObject.text;
        }
        else
        {
            Debug.LogError("Textオブジェクトが見つかりませんでした");
        }

    }
    public void OnEnable()
    {
        if (containerImage != null)
            normalColor = containerImage.color;
    }

    public void OnDrop(PointerEventData data)
    {
        containerImage.color = normalColor;

        if (receivingImage == null)
            return;

        if (receivingImage != null)
            Debug.Log("OnDrop " + GetDropSprite(data));
        GameManager.droppedExtraAbility.Add((GetDropSprite(data), myPositionName));


        Sprite dropSprite = GetDropSprite(data);
        if (dropSprite != null)
            receivingImage.overrideSprite = dropSprite;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (containerImage == null)
            return;

        Sprite dropSprite = GetDropSprite(data);
        if (dropSprite != null)
            containerImage.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (containerImage == null)
            return;

        containerImage.color = normalColor;
    }

    private Sprite GetDropSprite(PointerEventData data)
    {
        var originalObj = data.pointerDrag;
        if (originalObj == null)
            return null;

        var dragMe = originalObj.GetComponent<DragMe>();
        if (dragMe == null)
            return null;

        var srcImage = originalObj.GetComponent<Image>();
        if (srcImage == null)
            return null;

        return srcImage.sprite;
    }
}
