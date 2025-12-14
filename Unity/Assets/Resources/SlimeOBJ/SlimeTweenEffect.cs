using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SlimeTweenEffect : MonoBehaviour
{
    [Header("슬라임 트윈 설정")]
    [SerializeField] private float tweenDuration = 0.5f;
    [SerializeField] private float squashAmount = 0.7f;  // 찌그러지는 정도
    [SerializeField] private float stretchAmount = 1.3f; // 늘어나는 정도
    [SerializeField] private float bounceIntensity = 1.2f;

    [Header("효과를 적용할 슬라임 모델(자식)")]
    public Transform slimeModel;

    private Vector3 originalScale;
    private float growthPercent = 0.00f;
    private Sequence seq;
    private bool isTweening = false;

    void Start()
    {
        slimeModel = transform.childCount > 0 ? transform.GetChild(0) : null;
        originalScale = slimeModel.localScale;
    }

    /// <summary>
    /// 기본 슬라임 트윈 효과
    /// </summary>
    public void PlaySlimeTween()
    {
        if (isTweening) return;

        StartCoroutine(SlimeTweenCoroutine());
    }

    /// <summary>
    /// 점프 시 슬라임 효과
    /// </summary>
    public void PlayJumpEffect()
    {
        if (isTweening) return;

        DOTween.Kill(transform);
        isTweening = true;

        Sequence jumpSequence = DOTween.Sequence();

        // 1. 준비 동작 - 아래로 눌림
        jumpSequence.Append(slimeModel.DOScale(
            new Vector3(originalScale.x * stretchAmount, originalScale.y * squashAmount, originalScale.z * stretchAmount),
            tweenDuration * 0.2f
        ).SetEase(Ease.OutCubic));

        // 2. 점프 - 위로 늘어남
        jumpSequence.Append(slimeModel.DOScale(
            new Vector3(originalScale.x * squashAmount, originalScale.y * stretchAmount, originalScale.z * squashAmount),
            tweenDuration * 0.3f
        ).SetEase(Ease.OutQuad));

        // 3. 원래 크기로 복귀
        jumpSequence.Append(slimeModel.DOScale(originalScale, tweenDuration * 0.5f)
            .SetEase(Ease.OutBounce));

        jumpSequence.OnComplete(() => isTweening = false);
    }

    /// <summary>
    /// 착지 시 슬라임 효과
    /// </summary>
    public void PlayLandingEffect()
    {
        if (isTweening) return;

        DOTween.Kill(slimeModel);
        isTweening = true;

        Sequence landingSequence = DOTween.Sequence();

        // 착지 시 찌그러짐
        landingSequence.Append(slimeModel.DOScale(
            new Vector3(originalScale.x * stretchAmount, originalScale.y * squashAmount, originalScale.z * stretchAmount),
            tweenDuration * 0.15f
        ).SetEase(Ease.OutCubic));

        // 탄성으로 원래대로
        landingSequence.Append(slimeModel.DOScale(originalScale, tweenDuration * 0.35f)
            .SetEase(Ease.OutElastic, 1.2f, 0.6f));

        landingSequence.OnComplete(() => isTweening = false);
    }

    /// <summary>
    /// 잡혔을 때 슬라임 효과
    /// </summary>
    public void PlayGrabbedEffect()
    {
        if (isTweening) return;

        DOTween.Kill(slimeModel);
        isTweening = true;

        Sequence grabbedSequence = DOTween.Sequence();

        // 움츠러드는 효과
        grabbedSequence.Append(slimeModel.DOScale(originalScale * 0.9f, 0.1f)
            .SetEase(Ease.OutQuad));

        // 약간 진동하는 효과
        grabbedSequence.Append(slimeModel.DOPunchScale(Vector3.one * 0.1f, 0.3f, 3, 0.5f));

        grabbedSequence.OnComplete(() => isTweening = false);
    }

    /// <summary>
    /// 놓였을 때 슬라임 효과
    /// </summary>
    public void PlayReleasedEffect()
    {
        if (isTweening) return;

        DOTween.Kill(slimeModel);
        isTweening = true;

        Sequence releasedSequence = DOTween.Sequence();

        // 확장되는 효과
        releasedSequence.Append(slimeModel.DOScale(originalScale * 1.2f, 0.15f)
            .SetEase(Ease.OutQuad));

        // 원래 크기로 돌아감
        releasedSequence.Append(slimeModel.DOScale(originalScale, 0.25f)
            .SetEase(Ease.OutBack, 1.5f));

        releasedSequence.OnComplete(() => isTweening = false);
    }

    /// <summary>
    /// 맥박 같은 슬라임 효과 (Idle 상태)
    /// </summary>
    public void PlayIdlePulse()
    {
        if (isTweening) return;

        slimeModel.DOScale(originalScale * 1.05f, 1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// 모든 트윈 정지
    /// </summary>
    public void StopAllTweens()
    {
        DOTween.Kill(slimeModel);
        isTweening = false;
        slimeModel.localScale = originalScale;
    }

    /// <summary>
    /// 기본 슬라임 트윈 코루틴
    /// </summary>
    private IEnumerator SlimeTweenCoroutine()
    {
        isTweening = true;

        // 1단계: 눌려지는 효과
        yield return slimeModel.DOScale(
            new Vector3(originalScale.x * stretchAmount, originalScale.y * squashAmount, originalScale.z * stretchAmount),
            tweenDuration * 0.3f
        ).SetEase(Ease.OutCubic).WaitForCompletion();

        // 2단계: 튀어오르는 효과
        yield return slimeModel.DOScale(
            new Vector3(originalScale.x * squashAmount, originalScale.y * stretchAmount, originalScale.z * squashAmount),
            tweenDuration * 0.4f
        ).SetEase(Ease.OutQuad).WaitForCompletion();

        // 3단계: 안정화
        yield return slimeModel.DOScale(originalScale, tweenDuration * 0.3f)
            .SetEase(Ease.OutBack, bounceIntensity).WaitForCompletion();

        isTweening = false;
    }

    /// <summary>
    /// 원래 크기 재설정 (런타임에서 크기가 변경된 경우)
    /// </summary>
    public void SetOriginalScale(Vector3 newScale)
    {
        originalScale = newScale;
    }

    /// <summary>
    ///  먹이를 먹어 크기 증가
    /// </summary>
    /// <param name="value"> 크기 증가율 (예: 0.05 → 5% 증가) </param>
    public void Feeded(float value = 0.1f)
    {
        growthPercent += value;                     // 성장 목표 저장

        float currentScale = growthPercent;
        float popTarget = growthPercent + value;    // 확 커지는 목표
        float finalTarget = growthPercent;          // 안정 목표

        Debug.Log($"현재 배율 : {currentScale} | 목표 배율 {growthPercent}");

        if (seq != null && seq.IsActive())
        {
            seq.Kill();
        }


        // 본채에 적용
        seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => currentScale, x => {
            currentScale = x;
            transform.localScale = originalScale * (1 + currentScale);
        }, popTarget, 0.3f).SetEase(Ease.OutBack));

        seq.Append(DOTween.To(() => currentScale, x => {
            currentScale = x;
            transform.localScale = originalScale * (1 + currentScale);
        }, finalTarget, 0.6f).SetEase(Ease.OutElastic, 1.2f, 0.6f));
        seq.Play();

    }

    void OnDestroy()
    {
        DOTween.Kill(slimeModel);
    }
}