#ifndef EARTH_UTILS_INCLUDED
#define EARTH_UTILS_INCLUDED

/**
 * Convert the XYZ coordinates of the sphere to Latitude Longitude Height
 * coordinates. The units are conserved, so if the XYZ coordinates are in km,
 * then so will be the Height. An all zero input represents the center of the
 * Eath. The Y coordinate reprezents the height.
 */
float3 xyzllhSimple(float3 xyz) {
    return float3(
        atan2(xyz.z, sqrt(xyz.x * xyz.x + xyz.y * xyz.y)),
        atan2(xyz.x, xyz.y),
        length(xyz)
    );
}

/**
 * Maps the given value from the range [low1, high1] to the range [low2, high2]
 */
float map(float value, float low1, float high1, float low2, float high2) {
    return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
}

/**
 * Unity-accesible wrapper for the given function.
 */
void XYZToLLH_float(float3 xyz, out float3 llh) {
    llh = xyzllhSimple(xyz);
}

/**
 * Unity-accesible function to convert LLH to XY texture coordinates for
 * equirectangular textures. The Height parameter is unused.
 */
void LLHToEquirectangular_float(float3 llh, out float2 xy) {
    xy = float2(
        map(llh.y, -PI, PI, 0, 1),
        map(llh.x, -PI / 2, PI / 2, 0, 1));
}

/**
 * Unity-accesible function that uses smooth interpolation to calculate the
 * SDF opacity. If the value is outside the limits, then it returns 0 or 1,
 * otherwise it interpolates using the function x^2 * (3 - 2x).
 */
void SDFOpacity_float(float textureValue, float2 limits, out float opacity) {
    opacity = smoothstep(limits.x, limits.y, textureValue);
}

#endif
