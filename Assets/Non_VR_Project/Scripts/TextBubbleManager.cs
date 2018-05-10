using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBubbleManager : MonoBehaviour
{

    [SerializeField] private Transform TaserButton;
    [SerializeField] private Transform TaserBubble;
    [SerializeField] private Text TaserText;

    [SerializeField] private Transform CatcherButton;
    [SerializeField] private Transform CatcherBubble;
    [SerializeField] private Text CatcherText;

    [SerializeField] private Transform DrillerButton;
    [SerializeField] private Transform DrillerBubble;
    [SerializeField] private Text DrillerText;

    [SerializeField] private Transform TrapButton;
    [SerializeField] private Transform TrapBubble;
    [SerializeField] private Text TrapText;

    [SerializeField] private Transform KittiesBarButton;
    [SerializeField] private Transform KittiesBarBubble;
    [SerializeField] private Text KittiesBarText;

    [SerializeField] private Color onCol;
    [SerializeField] private Color outCol;




    // Use this for initialization
    private void Awake()
    {


    }
    void Start()
    {
        //newCol = TaserBubble.GetComponent<Image>().color;

        onCol = new Color(0, 0, 0, 0.5f);
        outCol = new Color(0, 0, 0, 0);

        TaserBubble.gameObject.SetActive(false);
        TaserText.gameObject.SetActive(false);
        CatcherBubble.gameObject.SetActive(false);
        CatcherText.gameObject.SetActive(false);
        DrillerBubble.gameObject.SetActive(false);
        DrillerText.gameObject.SetActive(false);
        TrapBubble.gameObject.SetActive(false);
        TrapText.gameObject.SetActive(false);
        KittiesBarBubble.gameObject.SetActive(false);
        KittiesBarText.gameObject.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {

    }
    // ------------------------------------------------- 1. Mouse events of Taser shooter
    public void MouseOverTaser()
    {
        TaserBubble.gameObject.SetActive(true);
        TaserText.gameObject.SetActive(true);
        TaserButton.GetComponent<Image>().color = onCol;
        //TaserText.GetComponent<Text>().text = "Taser Shooter\n" + "Spawn to drain\n"+ "your enemy's shooting energy!\n";

        TaserText.GetComponent<Text>().text = "shooting energy!\n" + "your enemy's\n" + "Spawn to drain\n" + "Taser Shooter\n";

    }
    public void MouseExitTaser()
    {

        TaserButton.GetComponent<Image>().color = outCol;
        TaserBubble.gameObject.SetActive(false);
        TaserText.gameObject.SetActive(false);
    }
    // ------------------------------------------------- 2. Mouse events of Catcher
    public void MouseOverCatcher()
    {
        CatcherBubble.gameObject.SetActive(true);
        CatcherText.gameObject.SetActive(true);
        CatcherButton.GetComponent<Image>().color = onCol;
        CatcherText.GetComponent<Text>().text = "1st colloum\n" + "2nd colloum\n";
    }
    public void MousExitCatcher()
    {
        CatcherBubble.gameObject.SetActive(false);
        CatcherText.gameObject.SetActive(false);
        CatcherButton.GetComponent<Image>().color = outCol;
    }
    // ------------------------------------------------- 3. Mouse events of Faling Driller
    public void MouseOverDriller()
    {
        DrillerBubble.gameObject.SetActive(true);
        DrillerText.gameObject.SetActive(true);
        DrillerButton.GetComponent<Image>().color = onCol;
        DrillerText.GetComponent<Text>().text = "1st colloum\n" + "2nd colloum\n";
    }
    public void MousExitdDriller()
    {
        DrillerBubble.gameObject.SetActive(false);
        DrillerText.gameObject.SetActive(false);
        DrillerButton.GetComponent<Image>().color = outCol;
    }
    // ------------------------------------------------- 4. Mouse events of Trap
    public void MouseOverTrap()
    {
        TrapBubble.gameObject.SetActive(true);
        TrapText.gameObject.SetActive(true);
        TrapButton.GetComponent<Image>().color = onCol;
        TrapText.GetComponent<Text>().text = "1st colloum\n" + "2nd colloum\n";
    }
    public void MousExitTrap()
    {
        TrapBubble.gameObject.SetActive(false);
        TrapText.gameObject.SetActive(false);
        TrapButton.GetComponent<Image>().color = outCol;
    }
    // ------------------------------------------------- 5. Mouse events of Kitties' bar
    public void MouseOverKittiesBar()
    {
        KittiesBarBubble.gameObject.SetActive(true);
        KittiesBarText.gameObject.SetActive(true);
        KittiesBarButton.GetComponent<Image>().color = onCol;
        KittiesBarText.GetComponent<Text>().text = "1st colloum\n" + "2nd colloum\n";
    }
    public void MousExitKittiesBar()
    {
        KittiesBarBubble.gameObject.SetActive(false);
        KittiesBarText.gameObject.SetActive(false);
        KittiesBarButton.GetComponent<Image>().color = outCol;
    }


}
