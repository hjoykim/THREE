const char* clearcoat_normal_fragment_begin =R"(
#ifdef CLEARCOAT

	vec3 clearcoatNormal = geometryNormal;

#endif
)";