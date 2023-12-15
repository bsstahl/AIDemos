namespace ADA2.Client.Extensions;

public static class VectorExtensions
{
    /// <summary>
    /// Returns the sum of the 2 specified vectors
    /// </summary>
    /// <param name="v1">The 1st vector to be used in the operation</param>
    /// <param name="v2">The 2nd vector to be used in the operation</param>
    /// <returns>A vector value representing the sum of the 2 points</returns>
    public static float[] Sum(this float[] v1, float[] v2)
    {
        var a1 = v1.ToArray();
        var a2 = v2.ToArray();
        if (a1.Length != a2.Length) throw new InvalidOperationException("Vector lengths must be equal");
        var result = new List<float>();
        for (int i = 0; i < a1.Length; i++)
            result.Add(a1[i] + a2[i]);
        return result.ToArray();
    }

    /// <summary>
    /// Returns the difference of the 2 specified vectors
    /// </summary>
    /// <param name="v1">The 1st vector to be used in the operation</param>
    /// <param name="v2">The 2nd vector to be used in the operation</param>
    /// <returns>A vector value representing the 2nd value subtracted from the 1st value</returns>
    public static float[] Difference(this float[] v1, float[] v2)
    {
        var a1 = v1.ToArray();
        var a2 = v2.ToArray();
        if (a1.Length != a2.Length) throw new InvalidOperationException("Vector lengths must be equal");
        var result = new List<float>();
        for (int i = 0; i < a1.Length; i++)
            result.Add(a1[i] - a2[i]);
        return result.ToArray();
    }

    /// <summary>
    /// Returns the dot product of the 2 specified vectors
    /// </summary>
    /// <param name="v1">The 1st vector to be used in the operation</param>
    /// <param name="v2">The 2nd vector to be used in the operation</param>
    /// <returns>A vector value representing the two vectors multiplied together</returns>
    public static float DotProduct(this float[] v1, float[] v2)
    {
        var a1 = v1.ToArray();
        var a2 = v2.ToArray();
        if (a1.Length != a2.Length) throw new InvalidOperationException("Vector lengths must be equal");
        var result = 0.0f;
        for (int i = 0; i < a1.Length; i++)
            result += (a1[i] * a2[i]);
        return result;
    }

    /// <summary>
    /// Normalize the specified vector to unit length
    /// </summary>
    /// <param name="value">The vector to be normalized</param>
    /// <returns>A vector of unit length</returns>
    public static float[] Normalize(this float[] value)
    {
        var mag = Magnitude(value);
        return value.Select(v => Convert.ToSingle(v / mag)).ToArray();
    }

    /// <summary>
    /// Calculates the length of a vector
    /// </summary>
    /// <param name="value">The vector whose length is to be determined</param>
    /// <returns>A <see cref="Single"/> representing the length of the vector</returns>
    public static float Magnitude(this float[] value)
    {
        return Convert.ToSingle(Math.Sqrt(value.Sum(v => (v * v))));
    }

    /// <summary>
    /// Returns the cosine similarity of the 2 specified vectors
    /// </summary>
    /// <param name="v1">The 1st vector to be used in the operation</param>
    /// <param name="v2">The 2nd vector to be used in the operation</param>
    /// <returns>A vector value representing the similarity of the two original vectors</returns>
    /// <remarks>Specific to GPT style vectors. That is, vectors must have the correct number of dimensions and be normalized to unit length.</remarks>
    public static float CosineSimilarity(this float[] v1, float[] v2)
    {
        var dotProduct = v1.DotProduct(v2);
        var magnitudeV1 = Magnitude(v1);
        var magnitudeV2 = Magnitude(v2);
        return Convert.ToSingle(dotProduct / (magnitudeV1 * magnitudeV1));
    }

    /// <summary>
    /// Returns the cosine distance between the 2 specified vectors
    /// </summary>
    /// <param name="v1">The 1st vector to be used in the operation</param>
    /// <param name="v2">The 2nd vector to be used in the operation</param>
    /// <returns>A vector value representing the distance between the two original vectors</returns>
    /// <remarks>Specific to GPT style vectors. That is, vectors must have the correct number of dimensions and be normalized to unit length.</remarks>
    public static float CosineDistance(this float[] v1, float[] v2)
    {
        return 1.0f - v1.CosineSimilarity(v2);
    }

}
