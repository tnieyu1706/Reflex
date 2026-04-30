Khung Dependency Injection siêu nhanh, tối giản và mạnh mẽ cho Unity (Bản Custom)

Reflex là một framework Dependency Injection (DI) cho Unity, giúp tách biệt việc khởi tạo đối tượng khỏi việc sử dụng chúng. Bản tùy chỉnh này mở rộng khả năng của Reflex gốc với các tính năng về LocalScope, Inject Source, và các Instantiate Extensions mạnh mẽ.

🌟 Các tính năng nổi bật (Bản Custom)

Ngoài các tính năng gốc (Source Generation, Immutable Container, High Performance), phiên bản này bao gồm các cải tiến quan trọng:

LocalScope (GameObject-level Container): Cho phép tạo container riêng cho một GameObject và các con của nó. Cực kỳ hữu ích cho các UI Panel phức tạp hoặc các hệ thống độc lập trong Scene.

InjectSource: Kiểm soát chính xác nguồn resolve dependency (Root, Scene, Local) thông qua Attribute.

Instantiate & Bind Extensions: Hỗ trợ khởi tạo Prefab/Component và thực hiện Injection ngay lập tức trước khi phương thức Awake() chạy.

Visual Installers: Các Installer chuyên biệt cho MonoBehaviour và ScriptableObject với giao diện chọn Contract (Interface/Base Class) trực quan trong Inspector.

Scene Hierarchy Inheritance: Thiết lập quan hệ cha-con giữa các Scene Container trực tiếp qua Inspector của ContainerScope.

🛠 Các thay đổi chính & Cách sử dụng

1. LocalScope - Container cấp GameObject

LocalScope hoạt động như một "Mini-Container" trong phân cấp Transform.

Nó tự động tạo một container con kế thừa từ container gần nhất (thường là Scene Container).

Tự động Inject cho bản thân và các GameObject con.

Cách dùng: Gắn component LocalScope vào một GameObject. Thêm các Installer vào chính GameObject đó để đăng ký binding riêng cho phân cấp này.

2. Chỉ định nguồn Injection (InjectSource)

Bạn có thể ghi đè logic resolve mặc định bằng cách chỉ định nguồn thông qua InjectAttribute:

public class MyService : MonoBehaviour
{
    // Lấy instance từ Container của Scene, ngay cả khi LocalScope có ghi đè binding này
    [Inject(Source = InjectSource.Scene)] 
    private ILogger _sceneLogger;

    // Chỉ tìm trong Root (Project) Container
    [Inject(Source = InjectSource.Root)] 
    private IConfig _globalConfig;
}


3. Khởi tạo đối tượng động (Extensions)

Giải quyết vấn đề dependency không sẵn sàng trong Awake. Các phương thức mở rộng mới đảm bảo injection hoàn tất trước khi đối tượng "thức tỉnh":

// Khởi tạo prefab và inject ngay lập tức
var enemy = container.InstantiateAndBind(enemyPrefab, spawnPoint);

// Thêm component và inject trước khi Awake chạy
var health = container.AddComponentAndBind<Health>(gameObject);


4. Installer trực quan (Generic Installers)

Sử dụng MonoBehaviourInstaller hoặc ScriptableObjectInstaller để đăng ký các instance có sẵn trong Scene hoặc Asset:

Kéo thả object vào danh sách Bindings.

Hệ thống sẽ tự động liệt kê tất cả các Interface và Class cha hợp lệ để bạn tích chọn (Contract).

5. Phân cấp Scene qua Inspector

Trong ContainerScope (SceneScope), bạn hiện có thể kéo thả một Scene Asset vào trường Parent Scene.

Điều này cho phép Scene hiện tại kế thừa toàn bộ dependency từ Scene cha đã định nghĩa.

Hỗ trợ mạnh mẽ cho kiến trúc Multi-scene (ví dụ: Scene "UI" kế thừa từ Scene "Core").

🎬 Thứ tự thực thi (Execution Order)

Bản custom điều chỉnh thứ tự thực thi để đảm bảo tính ổn định:

ContainerScope (Scene): -1,000,000,000 (Khởi tạo container cho Scene).

LocalScope: SceneContainerScope + 10 (Khởi tạo container cho GameObject sau Scene).

Các Component thông thường: Thực thi sau khi đã được Inject đầy đủ.

🔍 Chẩn đoán & Debug (Diagnosis)

Bản custom tích hợp sẵn hệ thống Diagnosis giúp ghi lại CallStack của nơi thực hiện Binding và nơi thực hiện Resolution.

Mở Window → Analysis → Reflex Debugger để xem chi tiết.

Kích hoạt bằng cách thêm symbol REFLEX_DEBUG trong Player Settings.

📜 License

Phiên bản này kế thừa giấy phép MIT. Mọi đóng góp dựa trên bản gốc của Gustavo Santos đều được hoan nghênh.
