using System;
using System.Collections.Generic;
using Dominos;
using Newtonsoft.Json.Linq;
using UnityEngine;
using static Global;

public class SeosonHandler : MonoBehaviour
{
    [SerializeField]
    private List<Seasons> seosonsList;

    private void Awake()
    {
        Seasons.DisableSeosons(seosonsList);
    }

    // Start is called before the first frame update
    void Start()
    {
        WebServiceManager.instance.APIRequest(WebServiceManager.instance.getEnvironments, Method.GET, null, null, OnSuccessfullySeosonDownload, OnFail, CACHEABLE.NULL, true, null);
    }

    private void OnFail(string msg)
    {
        Debug.LogError("OnFail: " + msg);
        new Seasons(SeosonalEnvironmentsEnum.general.ToString(), seosonsList);
    }

    private void OnSuccessfullySeosonDownload(JObject obj, long code)
    {
        if (ResponseStatus.Check(code))
        {
            Debug.Log("obj: " + obj.ToString());
            SeasonalEnvironments seasonalEnvironments = SeasonalEnvironments.FromJson(obj.ToString());
            new Seasons(seasonalEnvironments.seosonName, seosonsList);
        }
        else
        {
            Debug.Log( obj.ToString());
            OnFail("Some Error in Success.");
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}

[Serializable]
public class Seasons
{
    public SeosonalEnvironmentsEnum seosonalEnvironmentsEnum;
    public GameObject environment;

    public Seasons(string seasonName, List<Seasons> seosonsList)
    {
        Seasons season = seosonsList.Find(seoson => seoson.seosonalEnvironmentsEnum.Equals((SeosonalEnvironmentsEnum)Enum.Parse(typeof(SeosonalEnvironmentsEnum), seasonName)));
        season.environment.SetActive(true);
    }

    public static void DisableSeosons(List<Seasons> seasons)
    {
        foreach (var season in seasons)
        {
            season.environment.SetActive(false);
        }
    }
}