const char* metalnessmap_pars_fragment =R"(
#ifdef USE_METALNESSMAP

	uniform sampler2D metalnessMap;

#endif
)";