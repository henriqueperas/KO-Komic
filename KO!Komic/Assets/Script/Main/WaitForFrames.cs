using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForFrames : IEnumerator
{
    private int targetFrame;

    // Construtor: define quantos frames esperar
    public WaitForFrames(int frames)
    {
        targetFrame = Time.frameCount + frames;
    }

    // Interface obrigatória do IEnumerator
    public object Current => null;

    // Verifica se já atingiu o frame alvo
    public bool MoveNext()
    {
        return Time.frameCount < targetFrame;
    }

    public void Reset() { }
}
