using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeHandler : MonoBehaviour
{


    public void ChangeToBlack()
    {
       
        SpriteManager.Instance.currentColor = "Blue"; // Add this line
    }

    public void ChangeToRed()
    {
  
        SpriteManager.Instance.currentColor = "Red"; // Add this line
    }

    public void ChangeToWhite()
    {
     
        SpriteManager.Instance.currentColor = "White"; // Add this line
    }


}
