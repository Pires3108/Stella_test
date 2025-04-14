using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldUrl : MonoBehaviour
{
    public string instagram = "https://www.instagram.com/chalantos00/";
    public string youtube = "https://www.youtube.com/channel/UCThgj0_TjLzgddxPx0GosCA";
    public string itch = "https://chalantos.itch.io/";
    public string x = "https://x.com/chalantos";
    public string site = "https://sites.google.com/view/chalantos/home";

    public void AbrirURLInstagram()
    {
        Application.OpenURL(instagram);
    }

    public void AbrirURLYoutube()
    {
        Application.OpenURL(youtube);
    }

    public void AbrirURLITCHIO()
    {
        Application.OpenURL(itch);
    }

    public void AbrirURLXXX()
    {
        Application.OpenURL(x);
    }

    public void AbrirURLSite()
    {
        Application.OpenURL(site);
    }
}
