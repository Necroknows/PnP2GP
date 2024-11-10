using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;

    [SerializeField] TextMeshProUGUI textBox;
    [SerializeField] Animator animator;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        animator.SetBool("IsOpen", false);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            StopInteract();
        }
    }

    public void StopInteract()
    {
        animator.SetBool("IsOpen", false);
        textBox.text = string.Empty;
    }

    public void Interact(string _prompt)
    {
        if (_prompt != null)
        {
            textBox.text = _prompt;
            animator.SetBool("IsOpen", true);
        }
    }
}
