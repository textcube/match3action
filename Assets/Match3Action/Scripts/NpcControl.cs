using UnityEngine;
using System.Collections;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

/// <summary>
/// To draw damage motion with npc monster.
/// </summary>
public class NpcControl : MonoBehaviour {
	public GameObject bloodEffect;
	public UISlider hpBar;
	public UISprite idleSprite, damageSprite;
    SpriteRenderer sRender;
	
	float healthPoint = 1f;

    Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        sRender = GetComponent<SpriteRenderer>();
    }
    
    IEnumerator DoneDamage(float delayTime)
    {
		yield return new WaitForSeconds(delayTime);
        if (idleSprite) idleSprite.enabled = true;
        if (damageSprite) damageSprite.enabled = false;
	}

	IEnumerator DoDamage(float delayTime) {
        if (damageSprite) damageSprite.enabled = true;
        if (idleSprite) idleSprite.enabled = false;
		yield return new WaitForSeconds(delayTime);
        if (animator && animator.GetBool("Damage")) animator.SetBool("Damage", false);
        GameObject instance = NGUITools.AddChild(transform.parent.gameObject, bloodEffect);
        if (sRender)
            instance.transform.localPosition = transform.localPosition + new Vector3(-10f, 20f, 0f);
        else
            instance.transform.localPosition = transform.localPosition + new Vector3(-10f, 100f, 0f);
    }
	
	void SetHealthPoint(float point){
		if (point<0f) point = 1f;
		if (point>1f) point = 1f;
		TweenParms parms = new TweenParms().Prop("sliderValue", point).Ease(EaseType.EaseOutQuart);
		HOTween.To(hpBar, 0.1f, parms );
		healthPoint = point;
	}

	void SetHealthDamage(float damage){
		SetHealthPoint(healthPoint - damage);
	}

	public void Damage(){
        if (animator) animator.SetBool("Damage", true);
        StartCoroutine(DoDamage(0.1f));
		StartCoroutine( DoneDamage(0.1f) );
		SetHealthDamage(0.1f);
	}
}
