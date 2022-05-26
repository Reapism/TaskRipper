using System.Reflection;

namespace TaskRipper.Core
{
    public class DelegateInfoContext
    {
        public DelegateInfoContext(Delegate @delegate, object[]? args)
        {
            ReturnParameter = @delegate.Method.ReturnParameter;
            var i = 0;
            ParametersByOrder = @delegate.Method
                .GetParameters()
                .ToDictionary
                (
                    pi => i++,
                    pi => new TypeValue(pi, args[i])
                );
        }

        private void X()
        {

        }

        public ParameterInfo ReturnParameter { get; set; }
        internal IDictionary<int, TypeValue> ParametersByOrder { get; set; }

        public bool HasReturnType => !typeof(void).Equals(ReturnParameter.ParameterType);
        public bool HasParameters => ParametersByOrder != null && ParametersByOrder.Any();
    }
}
