using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusCotroller : MonoBehaviour
{
    // Start is called before the first frame update

    public float minWaitingTime;
    public float maxWaitingTime;

    private float _currWaitTime;
    private float _fromLastBonus;
    private List<Bonus> _bonuses;

    void Start()
    {
        _bonuses = new List<Bonus>();
        _fromLastBonus = 0;
        _currWaitTime = 0;
//        Bonus bonus = GenerateBonus();
//        _bonuses.Add(bonus);
        CreateRandomWaitingTime();
    }

    // Update is called once per frame
    void Update()
    {
        _fromLastBonus += Time.deltaTime;
        if (_fromLastBonus >= _currWaitTime)
        {
            Bonus bonus = GenerateBonus();
            _bonuses.Add(bonus);
            CreateRandomWaitingTime();
            _fromLastBonus = 0;
        }
    }

    void CreateRandomWaitingTime()
    {
        _currWaitTime = Random.Range(minWaitingTime, maxWaitingTime)*Time.deltaTime;
    }

    Bonus GenerateBonus()
    {
        int whichBonus = Random.Range(1, 3);
        Bonus bonus;
        if (whichBonus == 1)
        {
            bonus = Instantiate(Resources.Load<Bonus>("Bonus1"));
        }
        else
        {
            bonus = Instantiate(Resources.Load<Bonus>("Bonus2"));
        }
//        Bonus bonus = Instantiate(Resources.Load<Bonus>("Bonus"));
        bonus.Init();
        return bonus;
    }
}