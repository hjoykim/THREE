const char* fog_vertex =R"(
#ifdef USE_FOG

	fogDepth = - mvPosition.z;

#endif
)";