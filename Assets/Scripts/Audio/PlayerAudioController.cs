class PlayerAudioController : AudioController
{
    public AudioData MeleePreAttackAudio;
    public AudioData MeleeHitAttackAudio;
    public AudioData RangePreShotAudio;
    public AudioData RangeHitShotAudio;

    public void PlayMeleePreAttack() => PlayAudio(MeleePreAttackAudio);
    public void PlayMeleeHitAttack() => PlayAudio(MeleeHitAttackAudio);
    public void PlayRangePreShotAttack() => PlayAudio(RangePreShotAudio);
    public void PlayRangeHitShotAttack() => PlayAudio(RangeHitShotAudio);
}