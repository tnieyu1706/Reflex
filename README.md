🚀 Reflex DI (Custom Edition)

Ultra-fast, minimal, and powerful Dependency Injection for Unity

Reflex là một Dependency Injection (DI) framework tối giản, hiệu suất cao dành cho Unity.

Phiên bản tùy chỉnh (Custom Version) này được thiết kế lại kiến trúc lõi dựa trên bản gốc của Gustavo Santos để giải quyết các vấn đề phức tạp trong thực tế. Điểm nhấn của phiên bản này là khả năng thiết lập Parent Container linh hoạt, giải quyết triệt để bài toán Multi-scene bằng cách cho phép kéo thả trực tiếp (Drag & Drop) Parent Scene vào Container, cùng với hệ thống Lifecycle và Scope được tinh chỉnh hoàn toàn mới.

✨ Các Tính Năng Độc Quyền (Custom Version)

1. 🏗️ Kiến trúc Container & Multi-scene linh hoạt

Kiến trúc package đã được viết lại để việc thiết lập quan hệ cha-con (Parent) giữa các Container trở nên dễ dàng.
Trong ContainerScope (SceneScope), bạn có thể trực tiếp kéo thả một Scene Asset vào trường Parent Scene. Scene hiện tại sẽ kế thừa toàn bộ dependency từ Scene cha, hỗ trợ hoàn hảo cho cấu trúc Multi-scene (Ví dụ: Scene "UI" kế thừa toàn bộ dịch vụ từ Scene "Core").

2. 📦 LocalScope (Container cấp độ GameObject)

Hoạt động như một "Mini-Container" gắn trực tiếp vào một Transform.

Kế thừa thông minh: Tự động tạo một container con kế thừa từ container gần nhất (Scene hoặc LocalScope cha).

Tự động đệ quy: Tự động tiêm (inject) dependency vào chính nó và toàn bộ các GameObject con nằm trong nhánh phân cấp đó. Rất hữu ích cho các UI Panel hoặc hệ thống cô lập.

3. 🎯 Phạm vi & Phương thức Giải quyết (InjectScope & InjectResolutionMethod)

Bổ sung khả năng kiểm soát độ phân giải Inject chi tiết tới từng biến:

InjectScope (Phạm vi Inject): Cho phép ghi đè luồng tìm kiếm dependency mặc định.

InjectResolutionMethod (Phương thức giải quyết): Giải quyết bài toán bảo toàn giá trị cho các object [Serializable]. Thay vì Inject toàn bộ (làm mất các giá trị đã serialize trên Inspector), phương thức Binding chỉ tiêm vào các thuộc tính/trường được đánh dấu [Inject], bảo toàn nguyên vẹn cấu trúc còn lại.

public class MyService : MonoBehaviour
{
    // Xác định rõ phạm vi tìm kiếm dependency ở Scene hiện tại
    [Inject(Scope = InjectScope.Scene)] 
    private ILogger _sceneLogger;

    // Giữ an toàn cho các biến Serializable khác, chỉ inject biến này
    [Inject(ResolutionMethod = InjectResolutionMethod.Binding)] 
    private IConfig _globalConfig;
}


4. 🧩 Tách biệt cấu trúc Construct: Instantiate và InjectObject

Nhằm tăng tính linh hoạt khi sử dụng, quá trình Construct nguyên bản đã được tách thành 2 phương thức rõ rệt:

Sử dụng Inject trên Object có sẵn:

var myObj = new MyClass(); // Hoặc object lấy từ nơi khác
container.Inject(myObj);   // Chỉ thực hiện tiêm dependency


Sử dụng Instantiate qua Container:

// Khởi tạo và tiêm tự động
var myObj = container.Instantiate<MyClass>();


(Lưu ý: Khi dùng Instantiate, constructor của class phải được đánh dấu bằng Attribute [ReflexConstructor] và các tham số truyền vào hàm tạo phải là các tham số dùng để đón Inject).

Bên cạnh đó, các Extension như InstantiateAndBind hoặc AddComponentAndBind vẫn được hỗ trợ để đảm bảo dependency sẵn sàng trước khi Awake() của Unity chạy.

5. ⏳ Mở rộng Lifecycle với EarlyInjector

Thiết lập thêm một hệ thống EarlyInjector nhằm tạo ra một vòng đời (lifecycle) tiêm phòng rõ ràng hơn.
Hệ thống này cho phép khởi tạo và tiêm dependency vào các thành phần đặc thù trước khi các luồng inject cơ bản diễn ra. Đặc biệt hữu dụng để tiêm dependency vào các Scriptable Object (ngay sau khi chúng được load vào scene) hoặc các file cấu hình hệ thống cần độ ưu tiên cao nhất.

6. 🛠️ Templates hỗ trợ sẵn (Installer & Injector)

Không cần phải viết lại code hiển thị cho Inspector mỗi lần tạo Installer mới. Phiên bản này cung cấp sẵn các Template mạnh mẽ:

Cung cấp abstract class GenericInstaller<T>.

Hỗ trợ sẵn Custom Display cực kỳ trực quan trên Inspector. Bạn chỉ cần kế thừa lớp này, mọi tính năng kéo thả và chọn list Interface/Base Class (Contract) sẽ được hệ thống tự động xử lý.

🎬 Thứ Tự Thực Thi (Execution Order)

Hệ thống điều chỉnh lại thứ tự thực thi của Unity để đảm bảo tính ổn định tuyệt đối:

ContainerScope (Scene): -1,000,000,000 (Khởi tạo Scene container đầu tiên).

LocalScope: SceneContainerScopeExecutionOrder + 10 (Khởi tạo GameObject container ngay sau Scene Container).

EarlyInjector: Thực thi tiêm cho ScriptableObjects và các luồng ưu tiên.

Standard Components: Hoạt động bình thường với dependency đã sẵn sàng trước Awake().

🔍 Chẩn Đoán & Gỡ Lỗi (Diagnosis)

Tích hợp sẵn hệ thống Diagnosis chuyên sâu, ghi lại CallStack ở cả thời điểm Binding và Resolution.

Sử dụng: Window → Analysis → Reflex Debugger (Dạng Tree-view).

Kích hoạt: Thêm REFLEX_DEBUG vào Scripting Define Symbols trong Player Settings.

📜 Giấy Phép (License)

Dự án này sử dụng giấy phép MIT License. Xin gửi lời cảm ơn và sự tri ân đến Gustavo Santos - tác giả của framework Reflex nguyên bản. Mọi đóng góp (Pull Request) để phát triển thêm dựa trên phiên bản này đều luôn được chào đón!
