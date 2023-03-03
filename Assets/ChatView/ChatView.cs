using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatView : MonoBehaviour
{
    [SerializeField] private InputField InputText;
    [SerializeField] private ChatComponent ChatComponent;

    public void SetText() {
        ChatComponent.SetChatDesc(InputText.text);
    }
}
