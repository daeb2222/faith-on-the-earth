using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextIntroductionAnimation : MonoBehaviour
{
   

    public TextMeshProUGUI textMeshPro;
    public float typingSpeed = 0.05f;
    public string fullText;
    private bool isTextComplete = false;

    private void Start()
    {
        StartCoroutine(TypeText());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isTextComplete)
            {
                textMeshPro.text = fullText;
                isTextComplete = true;
            }
            else
            {
                ChangeScene();
            }
        }
    }
    private IEnumerator TypeText()
    {
        textMeshPro.text = "";

        foreach (char letter in fullText)
        {
            textMeshPro.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
    private void ChangeScene()
    {
        SceneManager.LoadScene("GameLoop");
    }

}
