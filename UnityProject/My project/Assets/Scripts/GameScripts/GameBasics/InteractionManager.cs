using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;
    public KeyCode activate;

    [SerializeField] TextMeshProUGUI textBox;
    [SerializeField] public Animator animator;


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
        if (Input.GetKeyUp(activate))
        {
            StopInteract();
        }
    }

    public void StopInteract()
    {
        animator.SetBool("IsOpen", false);
        textBox.text = string.Empty;
    }

    public void Interact(string _prompt, KeyCode key)
    {
        if (_prompt != null)
        {
            textBox.text = _prompt;
            animator.SetBool("IsOpen", true);
        }
        else
        {
            //Debug.Log("Prompt is empty");
        }
        if (key != KeyCode.None)
        {
            activate = key;
        }
        else
        {
            //Debug.Log("No activation Key for prompt");
        }
    }
}
