using System;
using System.Collections.Generic;
using Assets.Scripts.Entities;
using Assets.Scripts.Services.Database;
using Random = UnityEngine.Random;
namespace Assets.Scripts.Services
{
    public class OperatorDataHelper
    {
        private IGameDatabase _database;
        public Action OnOpDataChange; 
        public IList<Operator> Operators { get; }
        public OperatorDataHelper(IGameDatabase database)
        {
            _database = database;
            Operators = _database.Operators;
        }
        
        public Operator GachaSimpleOp()
        {
            Operator op;
            if (Random.Range(0f, 1f) < 0.1f) // 10 %的概率抽到CV
            {
                op = new Operator
                {
                    Name = "CV_" + MyConfig.NameList[Random.Range(0, MyConfig.NameList.Count)].ToString(),
                    ModelResourceUrl = _database.ModelList[Random.Range(0, _database.ModelList.Count)],
                    WeaponSkillId = 6,
                    Type = OperatorType.CV,
                    Id = (_database.Operators.Count + 1).ToString(),
                };
            }
            else
            {
                op = new Operator
                {
                    Name = "CA_" + MyConfig.NameList[Random.Range(0, MyConfig.NameList.Count)].ToString(),
                    ModelResourceUrl =_database.ModelList[Random.Range(0, _database.ModelList.Count)],
                    WeaponSkillId = 4,
                    Type = OperatorType.CA,
                    Id = (_database.Operators.Count + 1).ToString(),
                };
            }
            op.PropGreen = Random.Range(1, 4);
            op.PropRed = Random.Range(1, 4);
            op.PropBlue = Random.Range(1, 4);

            return op;
        }
        public Operator GachaExpensiveOp()
        {
            Operator op;
            if (Random.Range(0f, 1f) < 0.15f) // 15 %的概率抽到CV
            {
                op = new Operator
                {
                    Name = "CV_" + MyConfig.NameList[Random.Range(0, MyConfig.NameList.Count)].ToString(),
                    ModelResourceUrl = _database.ModelList[Random.Range(0, _database.ModelList.Count)],
                    WeaponSkillId = 6,
                    Type = OperatorType.CV,
                    Id = (MyServices.Database.Operators.Count + 1).ToString(),
                };
            }
            else
            {
                op = new Operator
                {
                    Name = "CA_" + MyConfig.NameList[Random.Range(0, MyConfig.NameList.Count)].ToString(),
                    ModelResourceUrl = _database.ModelList[Random.Range(0, _database.ModelList.Count)],
                    WeaponSkillId = 4,
                    Type = OperatorType.CA,
                    Id = (_database.Operators.Count + 1).ToString(),
                };
            }
            op.PropGreen = Random.Range(3, 8);
            op.PropRed = Random.Range(3, 8);
            op.PropBlue = Random.Range(3, 8);

            return op;
        }

        public void AddOperator(Operator op)
        {
            Operators.Add(op);
        }
    }
}