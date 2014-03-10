using UnityEngine;
using System.Collections;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

/// <summary>
/// To draw attack animation with player.
/// </summary>
public class PcControl : MonoBehaviour {
	public GameObject slashEffect;
	public UISlider hpBar;
	public UISprite idleSprite, attackSprite;
    SpriteRenderer sRender;
	
	float healthPoint = 1f;

    Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        sRender = GetComponent<SpriteRenderer>();
    }

	IEnumerator DoneAttack(float delayTime) {
		yield return new WaitForSeconds(delayTime);
        if (idleSprite) idleSprite.enabled = true;
		if (attackSprite) attackSprite.enabled = false;
	}

	IEnumerator DoAttack(float delayTime) {
        if (idleSprite) idleSprite.enabled = false;
        if (attackSprite) attackSprite.enabled = true;
		yield return new WaitForSeconds(delayTime);
        if (animator && animator.GetBool("Attack")) animator.SetBool("Attack", false);
        GameObject instance = NGUITools.AddChild(transform.parent.gameObject, slashEffect);
        if (sRender)
            instance.transform.localPosition = transform.localPosition + new Vector3(280, -80f, 0f);
        else
            instance.transform.localPosition = transform.localPosition + new Vector3(220, 80f, 0f);
    }
	
	void SetHealthPoint(float point){
		if (point<0f) point = 0f;
		if (point>1f) point = 1f;
		TweenParms parms = new TweenParms().Prop("sliderValue", point).Ease(EaseType.EaseOutQuart);
		HOTween.To(hpBar, 0.1f, parms );
		healthPoint = point;
	}

	void SetHealthDamage(float damage){
		SetHealthPoint(healthPoint - damage);
	}

	public void Attack(){
        if (animator) animator.SetBool("Attack", true);
        StartCoroutine(DoAttack(0.5f));
		StartCoroutine( DoneAttack(0.5f) );
	}
}
