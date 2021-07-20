const char* roughnessmap_pars_fragment =R"(
#ifdef USE_ROUGHNESSMAP

	uniform sampler2D roughnessMap;

#endif
)";