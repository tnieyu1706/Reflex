using System;

namespace Reflex.Attributes
{
    /// <summary>
    /// Đánh dấu một tham số trong Constructor để chỉ định Container phải tự động Inject giá trị vào.
    /// Nếu tham số không có attribute này, Container sẽ cố gắng tìm giá trị từ custom arguments truyền vào thủ công.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParamInjectAttribute : Attribute
    {
    }
}