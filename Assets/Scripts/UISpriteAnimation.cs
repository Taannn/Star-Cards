using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour
{
    public Image m_Image;
    public Sprite[] m_SpriteArray;
    public float m_Speed = .02f;
    private int m_IndexSprite;
    Coroutine m_CorotineAnim;
    bool IsDone;

    /// <summary>
    /// This script allow to make an animation with a spritesheet in a canvas image.
    /// </summary>
    public void Start()
    {
        IsDone = false;
        StartCoroutine(Func_PlayAnimUI());
    }

    /// <summary>
    /// Every m_Speed seconds, the next image is load in m_Image.
    /// </summary>
    /// <returns></returns>
    IEnumerator Func_PlayAnimUI()
    {
        yield return new WaitForSeconds(m_Speed);
        if (m_IndexSprite >= m_SpriteArray.Length)
        {
            m_IndexSprite = 0;
        }
        m_Image.sprite = m_SpriteArray[m_IndexSprite];
        m_IndexSprite += 1;
        if (IsDone == false)
            m_CorotineAnim = StartCoroutine(Func_PlayAnimUI());
    }
}
