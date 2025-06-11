namespace EntityService {
    public delegate float CPDelegate(ESObject self);
    /// <summary> Calculated Property </summary>
    public class CP {
        /// <summary> 계산을 실제로 수행하는 함수입니다. </summary>
        public CPDelegate m_cpCallback;

        ///<summary> 계산을 매번 하는 대신 계산된 결과를 저장해둡니다. </summary>
        public ESProperty m_cachedProp;

        ///<summary><see cref="m_cachedProp"/>의 값이 유효한지 아닌지를 나타냅니다. 유효하지 않다면 다시 계산해야 합니다.</summary>
        public bool m_isValid;

        public override string ToString()
        {
            return $"{m_cachedProp.GetString()} (v:{m_isValid})";
        }
    }
}
