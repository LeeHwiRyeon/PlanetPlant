namespace EntityService.Format {
    public class ESParseOptions {
        public enum ResolveAction {
            Merge,
            MergeIfExist, // Only for ClassResolver.
            Ignore,
            Replace,
        };

        public ResolveAction TableResolver = ResolveAction.Ignore;
        public ResolveAction ClassResolver = ResolveAction.Ignore;
        public ResolveAction PropertyResolver = ResolveAction.Ignore;
    }
}
