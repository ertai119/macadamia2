using UnityEngine;

public static class ParticleExtensions
{

    public static void Scale(this ParticleSystem particles, float scale, bool includeChildren = true) {
        ParticleScaler.Scale(particles, scale, includeChildren);
    }
}

public static class ParticleScaler
{
    static public void Scale(ParticleSystem particles, float scale, bool includeChildren = true)
    {
        ParticleSystem.MainModule main = particles.main;

        main.scalingMode = ParticleSystemScalingMode.Hierarchy;
        particles.transform.localScale = particles.transform.localScale * scale;
        //main.gravityModifierMultiplier = scale;
        if (includeChildren)
        {
            var children = particles.GetComponentsInChildren<ParticleSystem>();
            for (var i = children.Length; i-- > 0;) {
                if (children[i] == particles) { continue; }

                ParticleSystem.MainModule childModule = children[i].main;

                childModule.scalingMode = ParticleSystemScalingMode.Hierarchy;
                children[i].transform.localScale = children[i].transform.localScale * scale;
                //childModule.gravityModifierMultiplier = scale;
            }
        }
    }
}