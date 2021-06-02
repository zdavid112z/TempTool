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

float map2(float value, float2 interval1, float2 interval2) {
    return map(value, interval1.x, interval1.y, interval2.x, interval2.y);
}

void Map_float(float value, float2 fromMinMax, float2 toMinMax, out float result) {
    result = map(value, fromMinMax.x, fromMinMax.y, toMinMax.x, toMinMax.y);
}

void MapRad_float(float valueRad, float2 fromMinMaxDeg, float2 toMinMaxDeg, out float resultRad) {
    float2 fromMinMaxRad = fromMinMaxDeg * PI / 180.f;
    float2 toMinMaxRad = toMinMaxDeg * PI / 180.f;
    resultRad = map(valueRad, fromMinMaxRad.x, fromMinMaxRad.y, toMinMaxRad.x, toMinMaxRad.y);
}

void IsOutside01Any_float(float2 uv, out bool result)
{
    result = uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1;
}

void MapClamp_float(float value, float2 fromMinMax, float2 toMinMax, out float result) {
    result = clamp(map(value, fromMinMax.x, fromMinMax.y, toMinMax.x, toMinMax.y), toMinMax.x, toMinMax.y);
}

float toRad(float deg) {
    return deg * PI / 180.f;
}

float toDeg(float rad) {
    return rad / PI * 180.f;
}

float2 toRad2(float2 deg) {
    return deg * PI / 180.f;
}

float2 toDeg2(float2 rad) {
    return rad / PI * 180.f;
}

void ToRad_float(float angle, out float result) {
    result = angle * PI / 180.f;
}

void ToRad2_float(float2 angles, out float2 result) {
    result = angles * PI / 180.f;
}

bool isLonWrapping(float2 lonMinMax, float width, float epsilon) {
    float segmentLength = (lonMinMax.y - lonMinMax.x) / (width - 1);
    float diff = 360 - (lonMinMax.y - lonMinMax.x);
    return abs(segmentLength - diff) < epsilon;
}

void IsWrapping_float(float2 lonMinMax, float width, float epsilon, out bool isWrapping) {
    isWrapping = isLonWrapping(lonMinMax, width, epsilon);
}

/**
 * Unity-accesible wrapper for the given function.
 */
void XYZToLLH_float(float3 xyz, out float3 llh) {
    llh = xyzllhSimple(xyz);
}

float handleLonConversion(float2 lonMinMax, float lon) {
    if (lonMinMax.y > 180.0) {
        lon += 180.0;
        if (lon > 360.0) {
            lon -= 360.0;
        }
        return lon - 180;
    }
    return lon;
}

void HandleLonConversion_float(float2 lonMinMax, float lon, out float lonOut) {
    lonOut = handleLonConversion(lonMinMax, lon);
}

float lonTo180_180(float lon) {
    lon += PI;
    if (lon > 2 * PI)
        lon -= 2 * PI;
    return lon - PI;
}

float lonTo0_360(float lon) {
    if (lon < 0)
        return lon + PI * 2;
    return lon;
}

float LonToEquirectangular180(float lonRad, float2 lonMinMaxDeg, bool flipLon, bool isWrapping) {    
    float2 limits = toRad2(float2(-180, 180));
    float result;
    if (isWrapping)
        result = lonRad;
    else result = map2(lonRad, toRad2(lonMinMaxDeg), limits);
    if (flipLon)
        result = -result;
    return map(result, -PI, PI, 0, 1);
}

float LonToEquirectangular360(float lonRad, float2 lonMinMaxDeg, bool flipLon, bool isWrapping) {
    float2 limits = toRad2(float2(0, 360));
    float result;
    if (isWrapping)
        result = lonRad;
    else result = map2(lonRad, toRad2(lonMinMaxDeg), limits);
    if (flipLon)
        result = 2 * PI - result;
    return map(result, 0, 2 * PI, 0, 1);
}

float LatToEquirectangular(float latRad, float2 latMinMaxDeg, bool flipLat) {
    float result = map2(latRad, toRad2(latMinMaxDeg), toRad2(float2(-90, 90)));
    if (flipLat)
        result = -result;
    return map(result, -PI / 2, PI / 2, 0, 1);
}

void LLToEquirectangularCorrectedLon_float(float2 ll, float2 latMinMax, float2 lonMinMax, float width, bool flipLat, bool flipLon, out float2 llOut) {
    bool isWrapping = isLonWrapping(lonMinMax, width, 0.1);
    bool is360 = lonMinMax.y > 180;
    float x;
    if (is360)
        x = LonToEquirectangular360(lonTo0_360(ll.y), lonMinMax, flipLon, isWrapping);
    else x = LonToEquirectangular180(ll.y, lonMinMax, flipLon, isWrapping);
    float y = LatToEquirectangular(ll.x, latMinMax, flipLat);
    llOut = float2(x, y);
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
