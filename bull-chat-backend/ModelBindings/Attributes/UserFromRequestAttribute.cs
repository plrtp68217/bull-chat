using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace bull_chat_backend.ModelBindings.Attributes
{
    /// <summary>
    /// Атрибут, указывающий, что параметр метода должен быть привязан к объекту <see cref="User"/>,
    /// извлечённому из JWT в текущем HTTP-запросе.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class UserFromRequestAttribute : ModelBinderAttribute, IBindingSourceMetadata
    {
        /// <summary>
        /// Инициализирует новый экземпляр атрибута <see cref="UserFromRequestAttribute"/>.
        /// Указывает кастомный биндер и источник привязки.
        /// </summary>
        public UserFromRequestAttribute() : base(typeof(UserFromRequestModelBinder)) { }
        /// <summary>
        /// Указывает, что используется пользовательский источник привязки.
        /// </summary>
        BindingSource IBindingSourceMetadata.BindingSource => BindingSource.Custom;
    }
}
