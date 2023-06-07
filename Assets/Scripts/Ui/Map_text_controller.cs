using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Map_text_controller : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI SectionText;
    [SerializeField] TextMeshProUGUI RoomText;

    public void SetLocationText(Location location, int roomNumber)
    {
        switch (location)
        {
            case Location.location26:
                SectionText.text = "Location: " + "<color=#E56300>26</color>";
                RoomText.text = "Room: " + "<color=#E56300>26-" + roomNumber + "</color>";
                break;

            case Location.location71:
                SectionText.text = "Location: " + "<color=#BBE500>71</color>";
                RoomText.text = "Room: " + "<color=#BBE500>71-" + roomNumber + "</color>";
                break;

            case Location.location116:
                SectionText.text = "Location: " + "<color=#0FE500>116</color>";
                RoomText.text = "Room: " + "<color=#0FE500>116-" + roomNumber + "</color>";
                break;

            case Location.location161:
                SectionText.text = "Location: " + "<color=#00E59C>161</color>";
                RoomText.text = "Room: " + "<color=#00E59C>161-" + roomNumber + "</color>";
                break;

            case Location.location206:
                SectionText.text = "Location: " + "<color=#0082E5>206</color>";
                RoomText.text = "Room: " + "<color=#0082E5>206-" + roomNumber + "</color>";
                break;

            case Location.location251:
                SectionText.text = "Location: " + "<color=#2A00E5>251</color>";
                RoomText.text = "Room: " + "<color=#2A00E5>251-" + roomNumber + "</color>";
                break;

            case Location.locztion296:
                SectionText.text = "Location: " + "<color=#D600E5>296</color>";
                RoomText.text = "Room: " + "<color=#D600E5>296-" + roomNumber + "</color>";
                break;

            case Location.location341:
                SectionText.text = "Location: " + "<color=#E50049>341</color>";
                RoomText.text = "Room: " + "<color=#E50049>341-" + roomNumber + "</color>";
                break;

            case Location.locationCenter:
                SectionText.text = "Location: " + "<color=#2A00E5>251</color>";
                RoomText.text = "Room: " + "<color=#2A00E5>" + roomNumber + "</color>";
                break;
        }
    }
}
