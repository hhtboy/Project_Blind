﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blind
{
    public class UI_FieldScene : UI_Scene
    {
        private const int GAUGE_SIZE = 3;

        private int _currWaveGauge;
        private float _hp;
        private float _maxHp;
        private PlayerCharacter _player = null;
        private Slider[] _waveGauges = new Slider[GAUGE_SIZE];

        private const float ALPHA = 50f;
        private float _chargeAlpha = ALPHA;

        Coroutine _coCharge = null;
        
        enum Texts
        {
            Text_HP,
        }
        enum Images
        {
            Image_TestDamage,
            Image_TestHeal,

            Charge,
        }
        enum Sliders
        {
            Slider_WaveGauge1,
            Slider_WaveGauge2,
            Slider_WaveGauge3,
            Slider_HP,
        }
        protected override void Start()
        {
            EnemyCharacter monster = FindObjectOfType<EnemyCharacter>();
            if (monster != null)
            {
                Get<Image>((int)Images.Image_TestDamage).gameObject.BindEvent(() => monster.Hp.GetDamage(1.0f), Define.UIEvent.Click);
                Get<Image>((int)Images.Image_TestHeal).gameObject.BindEvent(() => monster.Hp.GetHeal(1.0f), Define.UIEvent.Click);
            }
        }
        public override void Init()
        {
            base.Init();
            Bind<Text>(typeof(Texts));
            Bind<Image>(typeof(Images));
            Bind<Slider>(typeof(Sliders));

            UIManager.Instance.KeyInputEvents -= HandleUIKeyInput;
            UIManager.Instance.KeyInputEvents += HandleUIKeyInput;

            _player = FindObjectOfType<PlayerCharacter>();
            _hp = _player.Hp.GetHP();
            _maxHp = _player.Hp.GetMaxHP();

            _waveGauges[0] = Get<Slider>((int)Sliders.Slider_WaveGauge1);
            _waveGauges[1] = Get<Slider>((int)Sliders.Slider_WaveGauge2);
            _waveGauges[2] = Get<Slider>((int)Sliders.Slider_WaveGauge3);
            OnWaveGaugeChanged(_player.CurrentWaveGauge);

            InitTexts();
            InitEvents();
        }
        private void InitTexts()
        {
            Get<Text>((int)Texts.Text_HP).text = $"{_hp}/{_maxHp}";
        }
        private void InitEvents()
        {
            _player.Hp.RefreshHpUI += OnHpChanged;
            _player.OnWaveGaugeChanged += OnWaveGaugeChanged;
            // Test
            Get<Image>((int)Images.Image_TestDamage).gameObject.BindEvent(() => _player.Hitted(1.0f), Define.UIEvent.Click);
            Get<Image>((int)Images.Image_TestHeal).gameObject.BindEvent(() => _player.Hp.GetHeal(1.0f), Define.UIEvent.Click);
            Get<Image>((int)Images.Image_TestHeal).gameObject.BindEvent(TestWaveGauge, Define.UIEvent.Click);
        }
        private void HandleUIKeyInput()
        {
            if (!Input.anyKey)
                return;

            if (_uiNum != UIManager.Instance.UINum)
            {
                //Debug.Log(_uiNum);
                //Debug.Log(UIManager.Instance.UINum);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // TODO 메뉴 UI
                Debug.Log("ESC");
                UIManager.Instance.ShowNormalUI<UI_Menu>();
            }
        }
        private void OnHpChanged(float hp, float maxHp)
        {
            _hp = hp;
            _maxHp = maxHp;
            Get<Text>((int)Texts.Text_HP).text = $"{_hp}/{_maxHp}";
            Get<Slider>((int)Sliders.Slider_HP).value = _hp / _maxHp;
        }
        private void OnWaveGaugeChanged(int gauge)
        {
            _currWaveGauge = gauge;
            int idx = 0;
            while (gauge >= 10)
            {
                _waveGauges[idx++].value = 1.0f;
                gauge -= 10;
            }
            if (idx >= GAUGE_SIZE) return;
            _waveGauges[idx].value = (float)gauge / 10.0f;
            for (int i = idx + 1; i < GAUGE_SIZE; i++)
                _waveGauges[i].value = 0f;
        }
        private void TestWaveGauge()
        {
            _player.CurrentWaveGauge += 1;
        }
        public void StartCharge()
        {
            _coCharge = StartCoroutine(CoStartCharge());
        }
        public void StopCharge()
        {
            if (_coCharge != null)
            {
                StopCoroutine(_coCharge);
                _coCharge = null;
            }
            _chargeAlpha = ALPHA;
            Get<Image>((int)Images.Charge).color = new Color(1f, 1f, 1f, _chargeAlpha / 255f);
        }
        IEnumerator CoStartCharge()
        {
            _chargeAlpha = ALPHA;
            while (true)
            {
                if (_chargeAlpha >= 255)
                {
                    _chargeAlpha = 255f;
                    Get<Image>((int)Images.Charge).color = new Color(1f, 1f, 1f, _chargeAlpha / 255f);
                    _coCharge = null;
                    break;
                }
                _chargeAlpha += 1.5f;
                Get<Image>((int)Images.Charge).color = new Color(1f, 1f, 1f, _chargeAlpha / 255f);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}

