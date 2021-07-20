const char* emissivemap_pars_fragment =R"(
#ifdef USE_EMISSIVEMAP

	uniform sampler2D emissiveMap;

#endif
)";