using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bow : MonoBehaviour
{
    [SerializeField]
    private VisualTreeAsset chargeBarAsset;
    private VisualElement chargeBarContainer;
    private VisualElement chargeBar;
    [SerializeField]
    private GameObject arrowPrefab;
    [SerializeField]
    private float force = 0;
    private float maxForce = 20f;
    private float stepForce = 20;

    // Start is called before the first frame update
    void Start()
    {
        chargeBarContainer = chargeBarAsset.Instantiate();
        UICenter.add(chargeBarContainer);
        chargeBar = chargeBarContainer.Q<VisualElement>("ChargeBar");
    }

    void OnGUI()
    {
        Vector2 barPos = Camera.main.WorldToScreenPoint(PlayerCenter.position);

        float barHeight = PlayerCenter.height * UICenter.ps2px;
        float playerWidth = PlayerCenter.width * UICenter.ps2px;

        chargeBar.style.width = barHeight * 0.8f * 0.25f;
        chargeBar.style.height = barHeight * 0.8f;
        chargeBar.style.left = barPos.x - playerWidth / 1.5f;
        chargeBar.style.top = Screen.height - barPos.y - barHeight + 10;

        chargeBar.Q<VisualElement>("Progress").style.height = Length.Percent(force / maxForce * 100);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 bowPos = transform.position;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        Vector2 dir = mousePos - bowPos;
        Debug.DrawLine(bowPos, mousePos);
        transform.right = dir;

        if (Input.GetKey(KeyCode.Space))
        {
            force = force + 0.5f * stepForce * Time.fixedDeltaTime;
            if (force >= maxForce)
            {
                force = maxForce;
                stepForce = stepForce * -1;
            }
            if(force <= 0)
            {
                force = 0;
                stepForce = stepForce * -1;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Shoot(dir);
        }
    }

    void Shoot (Vector2 dir)
    {
        GameObject newArrow = Instantiate(arrowPrefab, transform.position, transform.rotation);
        newArrow.GetComponent<Arrow>().height = transform.position.y - PlayerCenter.position.y;
        newArrow.GetComponent<Arrow>().startY = PlayerCenter.position.y;
        newArrow.GetComponent<Rigidbody2D>().velocity = transform.right * force;
        force = 0;
    }
}
