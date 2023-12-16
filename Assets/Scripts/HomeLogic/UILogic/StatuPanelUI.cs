using TMPro;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic
{
    internal class StatuPanelUI : HomeUIBase
    {

        [SerializeField]
        private TextMeshProUGUI m_populationTMP;
        [SerializeField]
        private TextMeshProUGUI m_electricTMP;
        [SerializeField]
        private TextMeshProUGUI m_ironTMP;
        [SerializeField]
        private TextMeshProUGUI m_ammoTMP;
        [SerializeField]
        private TextMeshProUGUI m_alTMP;
        [SerializeField]
        private TextMeshProUGUI m_gachaTMP;

        private void Start()
        {
            setPopulationText();
            setElectricText();
            setIronText();
            setAmmoText();
            setAlText();
            setGachaText();
            _context.HomeVM.Population.OnDataChange += setPopulationText;
            _context.HomeVM.ResElectric.OnDataChange += setElectricText;
            _context.HomeVM.ResIron.OnDataChange += setIronText;
            _context.HomeVM.ResAmmo.OnDataChange += setAmmoText;
            _context.HomeVM.ResAl.OnDataChange += setAlText;
            _context.HomeVM.ResGacha.OnDataChange += setGachaText;
        }
        private void OnDestroy()
        {
            _context.HomeVM.Population.OnDataChange -= setPopulationText;
            _context.HomeVM.ResElectric.OnDataChange -= setElectricText;
            _context.HomeVM.ResIron.OnDataChange -= setIronText;
            _context.HomeVM.ResAmmo.OnDataChange -= setAmmoText;
            _context.HomeVM.ResAl.OnDataChange -= setAlText;
            _context.HomeVM.ResGacha.OnDataChange -= setGachaText;
        }
        private void setPopulationText()
        {
            m_populationTMP.text = _context.HomeVM.Population.Data.ToString();
        }
        private void setElectricText()
        {
            m_electricTMP.text = _context.HomeVM.ResElectric.Data.ToString();
        }
        private void setIronText()
        {
            m_ironTMP.text = _context.HomeVM.ResIron.Data.ToString();
        }
        private void setAmmoText()
        {
            m_ammoTMP.text = _context.HomeVM.ResAmmo.Data.ToString();
        }
        private void setAlText()
        {
            m_alTMP.text = _context.HomeVM.ResAl.Data.ToString();
        }
        private void setGachaText()
        {
            m_gachaTMP.text = _context.HomeVM.ResGacha.Data.ToString();
        }
    }
}
