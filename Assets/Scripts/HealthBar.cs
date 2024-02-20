using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image imageFill;
    [SerializeField] private Vector3 offset;
    private float hp;
    private float maxHp;
    private float timer = 0;
    private float fillTarget;

    private Transform target;
    private void Update()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / 1);
        float newFillAmount = Mathf.Lerp(fillTarget, hp / maxHp, t);

        imageFill.fillAmount = newFillAmount;

        transform.position = target.position + offset;

        if (Mathf.Approximately(newFillAmount, hp / maxHp))
        {
            fillTarget = hp / maxHp;
            timer = 0;
        }
        //imageFill.fillAmount = Mathf.Lerp(imageFill.fillAmount, hp / maxHp, Time.deltaTime * 5f);
        transform.position = target.position + offset;
    }
    public void OnInit(float maxHp, Transform target)
    {
        this.maxHp = maxHp;
        this.target = target;
        hp = maxHp;
        imageFill.fillAmount = 1;
        fillTarget = hp / maxHp;
    }
    public void SetNewHp(float hp)
    {
        this.hp = hp;
    }
}
