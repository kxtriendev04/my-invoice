<template>
  <div class="declaration-container">
    <div class="page-content custom-scrollbar">
      <div class="paper-container">
        <RibbonText
          text="Phim hướng dẫn. Xem ngay!"
          actionText="Xem chi tiết"
          url="https://misa.vn/thong-bao"
        />

        <div class="form-title-section">
          <h1 class="form-title">
            TỜ KHAI<br />Đăng ký/Thay đổi thông tin sử dụng hóa đơn điện tử
          </h1>
          <div class="radio-group-center mt-12">
            <label class="radio-label">
              <input type="radio" v-model="formData.transType" :value="1" /> Đăng ký mới
            </label>
            <label class="radio-label">
              <input type="radio" v-model="formData.transType" :value="2" /> Thay đổi thông tin
            </label>
          </div>
          <p class="subtitle mt-8">
            Chọn "Đăng ký mới" tại lần đầu tiên đăng ký sử dụng HĐĐT.<br />
            Chọn "Thay đổi thông tin" khi có sự thay đổi thông tin đã đăng ký sử dụng với cơ quan
            thuế.
            <a href="#" class="link-blue">Xem hướng dẫn</a>
          </p>
        </div>

        <div class="form-section grid-layout overall-info mt-32">
          <div class="form-group grid-col-6">
            <label>Số:</label>
            <input type="text" v-model="formData.registrationNo" class="form-control" />
          </div>
          <div class="form-group grid-col-6">
            <label>Ngày lập:</label>
            <input type="date" v-model="formData.createdDate" class="form-control" />
          </div>

          <div class="form-group grid-col-6 mt-8">
            <label>Tên người nộp thuế:</label>
            <input type="text" v-model="formData.taxpayerName" class="form-control" />
          </div>

          <div class="form-group grid-col-12 mt-8">
            <label>Địa chỉ nộp thuế:</label>
            <div class="flex-align-center">
              <Info :size="14" class="icon-blue mr-4" />
              <span class="static-text">
                Tầng 9 Technosoft, Phường Cầu Giấy, Thành phố Hà Nội, Việt Nam
              </span>
            </div>
          </div>

          <div class="form-group grid-col-12 mt-8">
            <label>Mã số thuế:</label>
            <div class="mst-boxes">
              <input
                v-for="(char, index) in formData.taxCode.split('')"
                :key="index"
                type="text"
                :value="char"
                readonly
                class="mst-box text-center"
              />
            </div>
          </div>

          <div class="form-group grid-col-12 flex-align-center mt-8">
            <label class="w-auto">
              Cơ quan thuế quản lý:
              <Info :size="14" class="icon-blue inline-icon ml-4" />
            </label>
            <select class="form-control flex-1" v-model="formData.taxAuthority">
              <option value="Tây Ninh">Thuế cơ sở 1 tỉnh Tây Ninh</option>
              <option value="Hà Nội">Cục thuế TP Hà Nội</option>
            </select>
          </div>
        </div>

        <div class="alert-banner alert-yellow mt-24">
          <Info :size="16" class="icon-mr text-yellow-dark" />
          <div>
            <span>
              Thông tin <b>người đại diện pháp luật</b> cần <b>khớp đúng</b> với thông tin
              <b>Đăng ký doanh nghiệp/Đăng ký thuế</b>.
              <a href="#" class="link-blue">Xem hướng dẫn</a>
            </span>
            <br />
            <span class="text-gray-dark mt-4" style="display: block">
              Sau khi tờ khai được <b>chấp nhận</b>, cơ quan thuế sẽ gửi các thông báo quan trọng
              liên quan đến hóa đơn điện tử tới email/số điện thoại/địa chỉ này.
            </span>
          </div>
        </div>

        <div class="form-section representative-info mt-24">
          <div class="full">
            <label>Người liên hệ (Đại diện theo pháp luật/Hộ, cá nhân kinh doanh):</label>
            <input type="text" v-model="formData.representativeName" class="form-control" />
          </div>

          <div class="full mt-8">
            <label>Điện thoại liên hệ (Đại diện theo pháp luật/Hộ, cá nhân kinh doanh):</label>
            <input type="text" v-model="formData.contactPhone" class="form-control" />
          </div>

          <label class="mt-8">Số CC/CCCD/Số định danh:</label>
          <input type="text" v-model="formData.identityNo" class="form-control mt-8" />

          <label class="mt-8">Số hộ chiếu:</label>
          <input type="text" v-model="formData.passportNo" class="form-control mt-8" />

          <label class="mt-8">Quốc tịch:</label>
          <select class="form-control mt-8" v-model="formData.nationality">
            <option value="VN">Việt Nam</option>
          </select>

          <label class="mt-8">Ngày, tháng, năm sinh:</label>
          <input type="date" v-model="formData.birthDate" class="form-control mt-8" />

          <label class="mt-8">Giới tính:</label>
          <select class="form-control mt-8" v-model="formData.gender">
            <option :value="2">Nữ</option>
            <option :value="1">Nam</option>
          </select>
        </div>

        <div class="form-section grid-layout mt-16 nnt-info">
          <div class="form-group grid-col-8 flex-align-center">
            <label class="w-auto mr-8 label-address-nnt">
              Địa chỉ liên hệ của NNT:
              <Info :size="14" class="icon-blue inline-icon ml-4" />
            </label>
            <input
              type="text"
              v-model="formData.contactAddress"
              class="input-address-nnt form-control flex-1"
            />
          </div>

          <div class="form-group grid-col-8 flex-align-center mt-8">
            <label class="w-auto mr-8 label-address-nnt">Thư điện tử của NNT: </label>
            <input
              type="text"
              v-model="formData.email"
              class="input-address-nnt form-control flex-1"
            />
          </div>
        </div>

        <p class="legal-text bold mt-32 mb-16">
          Theo Nghị định số 123/2020/NĐ-CP ngày 19 tháng 10 năm 2020 của Chính phủ, chúng tôi/tôi
          thuộc đối tượng sử dụng hóa đơn điện tử. Chúng tôi/tôi đăng ký/thay đổi thông tin đã đăng
          ký với cơ quan thuế về việc sử dụng hóa đơn điện tử như sau:
        </p>

        <div class="options-section mt-24">
          <h4>Hình thức hóa đơn <a href="#" class="link-blue text-normal ml-8">? Hướng dẫn</a></h4>
          <div class="mt-12">
            <label class="checkbox-wrapper">
              <input type="checkbox" v-model="hinhThuc.coMa" />
              <span class="checkmark"></span> Có mã của cơ quan thuế
            </label>
            <label class="checkbox-wrapper">
              <input type="checkbox" v-model="hinhThuc.tuMayTinhTien" />
              <span class="checkmark"></span> Hóa đơn khởi tạo từ Máy tính tiền
            </label>
            <label class="checkbox-wrapper">
              <input type="checkbox" v-model="hinhThuc.khongMa" />
              <span class="checkmark"></span> Không có mã của cơ quan thuế
            </label>
          </div>

          <h4 class="mt-24">
            Hình thức gửi dữ liệu hóa đơn điện tử
            <a href="#" class="link-blue text-normal ml-8">? Hướng dẫn</a>
          </h4>
          <div class="alert-banner alert-yellow-light mt-12">
            <span class="bulb">💡</span> Vui lòng bỏ qua nếu bạn không thuộc đối tượng nào trong các
            đối tượng dưới đây.
          </div>

          <div class="nested-options mt-16">
            <label class="checkbox-wrapper bold text-black">
              <input type="checkbox" checked />
              <span class="checkmark"></span> a. Trường hợp sử dụng hóa đơn điện tử có mã...
            </label>
            <div class="indent-options">
              <label class="checkbox-wrapper">
                <input type="checkbox" />
                <span class="checkmark"></span> Doanh nghiệp nhỏ và vừa, hợp tác xã, hộ...
              </label>
              <label class="checkbox-wrapper">
                <input type="checkbox" />
                <span class="checkmark"></span> Doanh nghiệp nhỏ và vừa khác theo đề nghị...
              </label>
              <label class="checkbox-wrapper">
                <input type="checkbox" />
                <span class="checkmark"></span> Cơ quan thuế hoặc cơ quan được giao nhiệm vụ...
              </label>
            </div>

            <label class="checkbox-wrapper bold text-black mt-16">
              <input type="checkbox" checked />
              <span class="checkmark"></span> b. Trường hợp sử dụng hóa đơn điện tử không có mã...
            </label>
            <div class="indent-options">
              <label class="checkbox-wrapper">
                <input type="checkbox" />
                <span class="checkmark"></span> Gửi trực tiếp đến cơ quan thuế...
              </label>
              <label class="checkbox-wrapper">
                <input type="checkbox" checked />
                <span class="checkmark"></span> Gửi đến cơ quan thuế thông qua tổ chức cung cấp...
              </label>
            </div>
          </div>

          <h4 class="mt-24">
            Phương thức chuyển dữ liệu hóa đơn điện tử
            <a href="#" class="link-blue text-normal ml-8">? Hướng dẫn</a>
          </h4>
          <div class="mt-12">
            <label class="checkbox-wrapper">
              <input type="checkbox" checked />
              <span class="checkmark"></span> Chuyển đầy đủ nội dung từng hóa đơn.
            </label>
            <label class="checkbox-wrapper">
              <input type="checkbox" checked />
              <span class="checkmark"></span> Chuyển dữ liệu hóa đơn điện tử theo Bảng tổng hợp...
            </label>
          </div>

          <h4 class="mt-24">
            Loại hóa đơn sử dụng <a href="#" class="link-blue text-normal ml-8">? Hướng dẫn</a>
          </h4>
          <div class="grid-checkboxes mt-12">
            <label class="checkbox-wrapper">
              <input type="checkbox" checked />
              <span class="checkmark"></span> Hóa đơn GTGT
            </label>
            <label class="checkbox-wrapper">
              <input type="checkbox" checked />
              <span class="checkmark"></span> Hóa đơn bán hàng
            </label>
            <label class="checkbox-wrapper">
              <input type="checkbox" checked />
              <span class="checkmark"></span> Hóa đơn thương mại
            </label>
            <label class="checkbox-wrapper">
              <input type="checkbox" checked />
              <span class="checkmark"></span> Hóa đơn bán tài sản công
            </label>
            <label class="checkbox-wrapper">
              <input type="checkbox" checked />
              <span class="checkmark"></span> Hóa đơn bán hàng dự trữ quốc gia
            </label>
            <label class="checkbox-wrapper">
              <input type="checkbox" checked />
              <span class="checkmark"></span> Các loại hóa đơn khác
            </label>
            <label class="checkbox-wrapper">
              <input type="checkbox" checked />
              <span class="checkmark"></span> Các chứng từ được in, phát hành, sử dụng...
            </label>
          </div>
        </div>

        <div class="table-section mt-48">
          <div class="table-header">
            <h4>
              Danh sách chứng thư số sử dụng
              <a href="#" class="link-blue text-normal ml-8">? Hướng dẫn</a>
            </h4>
          </div>
          <div class="alert-banner alert-yellow-light mt-12">
            <span class="bulb">💡</span> Bạn cần cắm <b>"USB Token"</b> chứa chứng thư số trước khi
            chọn chứng thư số.
          </div>
          <table class="misa-table mt-12">
            <thead>
              <tr>
                <th width="50" class="text-center">STT</th>
                <th width="100" class="text-center">Chọn chứng thư</th>
                <th>Tên tổ chức...</th>
                <th>Số sê-ri chứng thư</th>
                <th colspan="2" class="text-center">Thời hạn sử dụng (Từ - Đến)</th>
                <th width="120">Hình thức đăng ký</th>
                <th width="80"></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, index) in tables.signatures" :key="index">
                <td class="text-center gray-text">{{ index + 1 }}</td>
                <td class="text-center">
                  <a
                    href="javascript:void(0)"
                    class="link-blue text-none-underline"
                    @click="handleElectronicSign"
                    >Chọn</a
                  >
                </td>
                <td><input type="text" v-model="item.organizationName" class="table-input" /></td>
                <td><input type="text" v-model="item.serialNumber" class="table-input" /></td>
                <td><input type="date" v-model="item.fromDate" class="table-input" /></td>
                <td><input type="date" v-model="item.toDate" class="table-input" /></td>
                <td>{{ item.action }}</td>
                <td class="text-center action-cells">
                  <Plus :size="16" class="icon-gray cursor-pointer" @click="addSignature" />
                  <Trash2 :size="16" class="icon-trash ml-8" @click="removeSignature(index)" />
                </td>
              </tr>
            </tbody>
          </table>
          <div class="table-actions mt-16">
            <button class="btn-outline" @click="addSignature">
              <Plus :size="14" class="mr-4" /> Thêm chứng thư số
            </button>
            <button class="btn-outline text-green ml-12">
              <PlayCircle :size="14" class="mr-4" /> Phim hướng dẫn
            </button>
          </div>
        </div>

        <div class="table-section mt-48">
          <div class="table-header">
            <h4>
              Tổ chức cung cấp dịch vụ hóa đơn điện tử
              <a href="#" class="link-blue text-normal ml-8">? Hướng dẫn</a>
            </h4>
          </div>
          <table class="misa-table mt-12">
            <thead>
              <tr>
                <th width="50" class="text-center">STT</th>
                <th>Tên tổ chức cung cấp dịch vụ</th>
                <th>Mã số thuế</th>
                <th colspan="2" class="text-center">Thời gian (Từ ngày - Đến ngày)</th>
                <th>Ghi chú</th>
                <th width="50" class="text-center"></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, index) in tables.tvans" :key="index">
                <td class="text-center gray-text">{{ index + 1 }}</td>
                <td><input type="text" v-model="item.name" class="table-input" /></td>
                <td><input type="text" v-model="item.taxCode" class="table-input" /></td>
                <td><input type="text" v-model="item.fromDate" class="table-input" /></td>
                <td><input type="text" v-model="item.toDate" class="table-input" /></td>
                <td><input type="text" v-model="item.note" class="table-input" /></td>
                <td class="text-center"><Plus :size="16" class="icon-gray cursor-pointer" /></td>
              </tr>
            </tbody>
          </table>
          <button class="btn-outline mt-12"><Plus :size="14" class="mr-4" /> Thêm dòng</button>
        </div>

        <div class="table-section mt-48">
          <div class="table-header">
            <h4>
              Thông tin đơn vị truyền nhận hóa đơn điện tử
              <a href="#" class="link-blue text-normal ml-8">? Hướng dẫn</a>
            </h4>
          </div>
          <table class="misa-table mt-12">
            <thead>
              <tr>
                <th width="50" class="text-center">STT</th>
                <th>Tên đơn vị truyền nhận</th>
                <th>Mã số thuế</th>
                <th colspan="2" class="text-center">Thời gian (Từ ngày - Đến ngày)</th>
                <th>Ghi chú</th>
                <th width="50" class="text-center"></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, index) in tables.transmissionUnits" :key="index">
                <td class="text-center gray-text">{{ index + 1 }}</td>
                <td><input type="text" v-model="item.name" class="table-input" /></td>
                <td><input type="text" v-model="item.taxCode" class="table-input" /></td>
                <td><input type="text" v-model="item.fromDate" class="table-input" /></td>
                <td><input type="text" v-model="item.toDate" class="table-input" /></td>
                <td><input type="text" v-model="item.note" class="table-input" /></td>
                <td class="text-center"><Plus :size="16" class="icon-gray cursor-pointer" /></td>
              </tr>
            </tbody>
          </table>
          <button class="btn-outline mt-12">
            <Plus :size="14" class="mr-4" /> Thêm đơn vị truyền nhận
          </button>
        </div>

        <div class="signature-section mt-48">
          <p class="bold">
            Chúng tôi cam kết hoàn toàn chịu trách nhiệm trước pháp luật về tính chính xác, trung
            thực của nội dung nêu trên và thực hiện theo đúng quy định của pháp luật.
          </p>
          <div class="signature-layout mt-24">
            <div class="sig-left flex-align-center">
              <label class="w-auto mr-8">Nơi lập:</label>
              <input
                type="text"
                v-model="formData.issuePlace"
                class="form-control"
                style="width: 200px"
              />
            </div>
            <div class="sig-right text-center">
              <p>Hà Nội, ngày 18/03/2026</p>
              <p class="bold mt-8">NGƯỜI NỘP THUẾ HOẶC ĐẠI DIỆN HỢP PHÁP CỦA NGƯỜI NỘP THUẾ</p>
              <p class="gray-text italic mt-4">(Chữ ký số, chữ ký điện tử của người nộp thuế)</p>
              <div v-if="tables.signatures.length > 0" class="signature-valid-box mt-16">
                <div class="watermark-bg">
                  <svg
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    stroke-width="3"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    class="check-icon"
                  >
                    <polyline points="20 6 9 17 4 12"></polyline>
                  </svg>
                </div>

                <div class="signature-content">
                  <div class="sig-title">Signature Valid</div>
                  <div class="sig-row">
                    <i>Ký bởi:</i>
                    <strong>{{ tables.signatures[0]?.signerName || 'Công ty CP TEST' }}</strong>
                  </div>
                  <div class="sig-row">
                    <i>Ký ngày:</i>
                    <strong>{{ tables.signatures[0]?.signDate || '27/03/2026' }}</strong>
                  </div>
                </div>
              </div>

              <!-- <button v-else class="btn-sign mt-16" :disabled="isSigning || isViewMode">
                <Edit2 :size="14" class="mr-8" />
                <span>{{ isSigning ? 'Đang đọc Token...' : 'Ký điện tử' }}</span>
              </button> -->
              <button v-else class="btn-sign" style="margin-top: 12px" disabled="true">
                Nhấn Lưu và gửi để ký điện tử
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="page-footer">
      <div class="footer-left">
        <button class="btn-text"><Printer :size="16" class="mr-4" /> In</button>
        <button class="btn-text ml-16">
          <Download :size="16" class="mr-4" /> Xuất khẩu <ChevronDown :size="14" class="ml-4" />
        </button>
      </div>
      <div class="footer-right">
        <button class="btn-outline text-blue border-blue">
          <CheckCircle :size="14" class="mr-4" /> Hỗ trợ rà soát thông tin tờ khai
        </button>
        <!-- <button
          class="btn-primary ml-12"
          @click="handleSignAndPublishInvoice"
          :disabled="isInvoiceSigning"
        >
          <Edit2 :size="14" class="mr-8" />
          <span>{{ isInvoiceSigning ? 'Đang ký...' : 'Ký phát hành hóa đơn' }}</span>
        </button> -->
        <button class="btn-cancel">Hủy</button>
        <!-- <button class="btn-cancel" @click="handleSaveDraft" :disabled="isViewMode">Lưu</button> -->
        <button class="btn-primary" @click="submitForm" :disabled="isViewMode">Lưu và gửi</button>
      </div>
    </div>

    <!-- Popup kết quả từ CQT -->
    <MSAlert
      :is-show="resultAlert.show"
      :title="resultAlert.title"
      @close="resultAlert.show = false"
    >
      <template #message>
        <p>{{ resultAlert.message }}</p>
      </template>
      <template #footer>
        <button class="btn-primary" @click="resultAlert.show = false">Đóng</button>
      </template>
    </MSAlert>
  </div>
</template>

<script setup>
import { useRoute, useRouter } from 'vue-router'
import { reactive, onMounted, onUnmounted, ref } from 'vue'
import {
  Info,
  Plus,
  Trash2,
  PlayCircle,
  Edit2,
  Printer,
  Download,
  ChevronDown,
  CheckCircle,
} from 'lucide-vue-next'
import RibbonText from '@/components/ui/RibbonText.vue'
import MSAlert from '@/components/ms-alert/MSAlert.vue'
import invoiceRegistrationApi from '@/api/invoiceRegistrationApi'
import invoicesApi from '@/api/invoicesApi'
import * as signalR from '@microsoft/signalr'
import { mockCertificates } from '@/data/mockCertificates'
import { useToastStore } from '@/stores/useToastStore'

const route = useRoute()
const router = useRouter()
const toast = useToastStore()

// State cho chế độ xem (readonly)
const isViewMode = ref(false)
const currentRegistrationId = ref(null)

// Hàm load dữ liệu tờ khai từ API
const loadRegistrationData = async (id) => {
  try {
    const res = await invoiceRegistrationApi.getById(id)
    const data = res?.data || res

    if (data) {
      // Cập nhật UI hình thức dựa trên invoiceAppType từ database
      hinhThuc.coMa = data.invoiceAppType === 1
      hinhThuc.khongMa = data.invoiceAppType === 2
      hinhThuc.tuMayTinhTien = data.invoiceAppType === 3

      // Gán dữ liệu vào formData
      Object.assign(formData, {
        transType: data.transType || 1,
        registrationNo: data.registrationNo || '',
        createdDate: data.createdDate ? data.createdDate.split('T')[0] : '',
        taxpayerName: data.taxpayerName || '',
        taxCode: data.taxCode || '',
        taxAuthority: data.taxAuthorityCode || data.taxAuthority || 'Hà Nội',
        representativeName: data.representativeName || '',
        contactPhone: data.contactPhone || '',
        identityNo: data.identityNo || '',
        passportNo: data.passportNo || '',
        nationality: data.nationality || 'VN',
        birthDate: data.birthDate ? data.birthDate.split('T')[0] : '',
        gender: data.gender || 2,
        contactAddress: data.contactAddress || '',
        email: data.contactEmail || '',
        issuePlace: data.issuePlace || 'Hà Nội',
        invoiceAppType: data.invoiceAppType || 1,
        dataTransferMode: data.dataTransferMode || 1,
      })

      // Kiểm tra nếu đã gửi (status = 4) thì chỉ xem
      if (data.status === 4) {
        isViewMode.value = true
      }

      // Map danh sách chứng thư số từ database vào UI
      tables.signatures = data.signatures || data.Signatures || []
    }
  } catch (error) {
    console.error('Lỗi tải dữ liệu tờ khai:', error)
    alert('Không thể tải dữ liệu tờ khai.')
  }
}

// 1. KHỞI TẠO STATE RỖNG HOẶC MẶC ĐỊNH CƠ BẢN
const formData = reactive({
  transType: 1,
  registrationNo: '',
  createdDate: '',
  taxpayerName: '',
  taxCode: '',
  taxAuthority: 'Hà Nội',
  representativeName: '',
  contactPhone: '',
  identityNo: '',
  passportNo: '',
  nationality: 'VN',
  birthDate: '',
  gender: 2,
  contactAddress: '',
  email: '',
  issuePlace: 'Hà Nội',
  invoiceAppType: 1, // Mặc định có mã
  dataTransferMode: 1, // Mặc định trực tiếp
})

// State bổ sung cho UI
const hinhThuc = reactive({
  coMa: true,
  tuMayTinhTien: false,
  khongMa: false,
})

const tables = reactive({
  signatures: [],
  tvans: [],
  transmissionUnits: [],
})

const isSigning = ref(false)
const isInvoiceSigning = ref(false) // State cho việc ký hóa đơn

// State cho Popup kết quả
const resultAlert = reactive({
  show: false,
  title: 'Thông báo từ Cơ quan Thuế',
  message: '',
})

// 2. HÀM GÁN GIÁ TRỊ MẶC ĐỊNH KHI MỞ FORM
const initDefaultValues = () => {
  // Lấy ngày hiện tại (Format: YYYY-MM-DD)
  const today = new Date().toISOString().split('T')[0]

  // Bạn có thể call API lấy thông tin công ty đang đăng nhập ở đây rồi gán vào
  Object.assign(formData, {
    registrationNo: 'TK' + Math.floor(Math.random() * 1000000), // Ví dụ random số tờ khai
    createdDate: today,
    taxpayerName: 'Công ty Cổ phần Đại Việt',
    taxCode: '0101234150996',
    representativeName: 'Nguyễn Đức Anh',
    contactPhone: '0326995333',
    birthDate: '1995-06-15',
    gender: 2,
    contactAddress: 'Thửa đất số 10362, tờ bản đồ số 1...',
    email: 'Catherine@importeet.com',
  })

  // Đổ data mặc định cho bảng (nếu cần)
  tables.signatures = []
}

// --- LOGIC KÝ SỐ ---
const handleElectronicSign = async () => {
  isSigning.value = true

  // Giả lập việc người dùng chọn chứng thư số từ danh sách cấu hình
  setTimeout(() => {
    // Hiển thị một danh sách đơn giản bằng prompt để chọn id (Fake UI chọn chứng thư)
    const certListStr = mockCertificates
      .map((c) => `${c.id}: ${c.subject} (${c.issuer})`)
      .join('\n')
    const selectedId = window.prompt(`CHỌN CHỨNG THƯ SỐ ĐỂ KÝ:\n${certListStr}`, '1')

    const cert = mockCertificates.find((c) => c.id === selectedId)

    if (cert) {
      const signature = {
        organizationName: cert.subject,
        serialNumber: cert.serial,
        fromDate: cert.fromDate,
        toDate: cert.toDate,
        actionType: 1,
        signerName: cert.signerName,
        signDate: new Date().toLocaleDateString('vi-VN'),
      }
      tables.signatures = [signature]
      toast.success('Ký điện tử thành công bằng chứng thư: ' + cert.subject)
    } else {
      toast.warning('Bạn chưa chọn chứng thư hoặc ID không hợp lệ.')
    }
    isSigning.value = false
  }, 800)
}

// 3. LOGIC VALIDATE & SUBMIT FORM
const submitForm = async () => {
  // --- A. Validate cơ bản (Có thể dùng thư viện VeeValidate/Yup để xịn hơn) ---
  if (!formData.registrationNo) {
    toast.warning('Vui lòng nhập Số tờ khai!')
    return
  }
  if (!formData.taxpayerName) {
    toast.warning('Vui lòng nhập Tên người nộp thuế!')
    return
  }
  if (!formData.taxCode || formData.taxCode.length < 10) {
    toast.warning('Mã số thuế không hợp lệ!')
    return
  }
  // if (tables.signatures.length === 0) {
  //   alert('Vui lòng thêm ít nhất 1 chứng thư số!')
  //   return
  // }

  // Đồng bộ invoiceAppType từ UI checkbox trước khi gửi
  if (hinhThuc.coMa) formData.invoiceAppType = 1
  else if (hinhThuc.khongMa) formData.invoiceAppType = 2
  else if (hinhThuc.tuMayTinhTien) formData.invoiceAppType = 3

  // --- B. Đóng gói Payload gửi API ---
  const user = JSON.parse(localStorage.getItem('user') || '{}')
  const company = JSON.parse(localStorage.getItem('company') || '{}')

  try {
    if (currentRegistrationId.value) {
      // Chế độ SỬA: chỉ cập nhật entity chính qua endpoint PUT
      const updatePayload = {
        ...formData,
        companyId: company.companyId || company.CompanyId,
        contactEmail: formData.email,
        status: 2,
      }

      await invoiceRegistrationApi.update(currentRegistrationId.value, updatePayload)
      toast.success('Lưu và gửi tờ khai thành công')
      router.push('/invoice/declaration/list')
      return
    }

    // Chế độ TẠO MỚI: gọi saveFull để lưu trọn bộ (registration + signatures...)
    const payload = {
      registration: {
        ...formData,
        registrationId: undefined,
        companyId: company.companyId || company.CompanyId,
        contactEmail: formData.email,
        status: 2, // Trạng thái: Đã ký
      },
      signatures: tables.signatures.map((s) => ({
        ...s,
        registrationId: '00000000-0000-0000-0000-000000000000',
      })),
      tvans: [...tables.tvans],
      transmissionUnits: [...tables.transmissionUnits],
    }

    const res = await invoiceRegistrationApi.saveFull(payload)
    toast.success(res?.userMsg || 'Lưu và gửi tờ khai thành công')
    router.push('/invoice/declaration/list')
  } catch (error) {
    toast.error('Lỗi: ' + (error.response?.data?.userMsg || error.message))
  }
}

// Lưu tờ khai (chỉ lưu chỉnh sửa, không gửi CQT)
const handleSaveDraft = async () => {
  try {
    // Đồng bộ invoiceAppType từ checkboxes UI trước khi lưu
    if (hinhThuc.coMa) formData.invoiceAppType = 1
    else if (hinhThuc.khongMa) formData.invoiceAppType = 2
    else if (hinhThuc.tuMayTinhTien) formData.invoiceAppType = 3

    // Chuẩn bị payload tương tự submitForm nhưng trạng thái là draft (ví dụ: 1)
    const company = JSON.parse(localStorage.getItem('company') || '{}')

    const payload = {
      registration: {
        ...formData,
        registrationId: currentRegistrationId.value || undefined,
        companyId: company.companyId || company.CompanyId,
        contactEmail: formData.email,
        status: 1, // Trạng thái draft
      },
      signatures: tables.signatures.map((s) => ({
        ...s,
        registrationId: currentRegistrationId.value || '00000000-0000-0000-0000-000000000000',
      })),
      tvans: [...tables.tvans],
      transmissionUnits: [...tables.transmissionUnits],
    }

    if (!currentRegistrationId.value) {
      // Nếu là tạo mới thì gọi saveFull để lưu cả bộ
      const res = await invoiceRegistrationApi.saveFull(payload)
      toast.success(res?.userMsg || 'Lưu tờ khai thành công')
      // Nếu backend trả về ID, cập nhật local state (cố gắng tìm nhiều trường khả dĩ)
      const newId =
        res?.data?.registration?.registrationId ||
        res?.registration?.registrationId ||
        res?.data?.registrationId ||
        null
      if (newId) currentRegistrationId.value = newId
    } else {
      // Nếu đã có ID thì gọi update (cập nhật đơn thuần)
      await invoiceRegistrationApi.update(currentRegistrationId.value, formData)
      toast.success('Lưu tờ khai thành công')
    }
  } catch (err) {
    console.error('Lỗi khi lưu tờ khai:', err)
    toast.error('Lỗi khi lưu tờ khai. Xem console để biết chi tiết.')
  }
}

// 4. METHODS CHO BẢNG
const addSignature = () => {
  tables.signatures.push({
    organizationName: '',
    serialNumber: '',
    fromDate: '',
    toDate: '',
    actionType: 1,
  })
}

const removeSignature = (index) => {
  if (tables.signatures.length > 1) {
    tables.signatures.splice(index, 1)
  }
}

// --- LOGIC SIGNALR ---
let connection = null

const startSignalR = async () => {
  const token = localStorage.getItem('token')
  connection = new signalR.HubConnectionBuilder()
    .withUrl('https://localhost:7248/notificationHub', {
      accessTokenFactory: () => token, // Phải truyền token nếu Hub yêu cầu Authorize
    })
    .withAutomaticReconnect()
    .build()
  connection.on('ReceiveRegistrationResult', (data) => {
    // Khi Server gửi lệnh "ReceiveRegistrationResult", popup sẽ hiện lên
    resultAlert.message = data.message
    resultAlert.show = true
  })

  try {
    await connection.start()
    console.log('SignalR Connected.')
  } catch (err) {
    console.error('SignalR Connection Error: ', err)
  }
}

// 5. LIFECYCLE HOOKS
onMounted(() => {
  const registrationId = route.params.id

  if (registrationId && registrationId !== 'create') {
    // Đang xem tờ khai cũ - load dữ liệu từ API
    currentRegistrationId.value = registrationId
    loadRegistrationData(registrationId)
  } else {
    // Tạo mới
    initDefaultValues()
    tables.signatures = []
    localStorage.removeItem('temporary_signature')
  }
  startSignalR()
})

onUnmounted(() => {
  if (connection) connection.stop()
})
</script>

<style scoped>
/* ==========================================
   RESET & LAYOUT CHÍNH (FIX SCROLL)
   ========================================== */
* {
  box-sizing: border-box;
}

.declaration-container {
  display: flex;
  flex-direction: column;
  height: 100vh; /* Khóa chiều cao bằng màn hình */
  width: 100%;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  color: #111827;
  font-size: 13px;
  background-color: #f0f2f4;
  min-height: 0; /* Quan trọng cho Flexbox */
}

.page-content {
  flex: 1; /* Chiếm toàn bộ không gian còn lại */
  overflow-y: auto; /* Chỉ cuộn ở khu vực này */
  padding: 24px;
  min-height: 0;
}

.paper-container {
  background: #fff;
  margin: 0 auto;
  padding: 24px 32px;
  width: 100%;
  max-width: 900px;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.05);
}

.page-footer {
  background: #fafafa;
  border-top: 1px solid #e5e7eb;
  padding: 12px 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 60px;
  flex-shrink: 0; /* Không cho footer co lại */
  z-index: 10;
}

/* ==========================================
   UTILITIES
   ========================================== */
.flex-1 {
  flex: 1;
}
.w-auto {
  width: auto;
}
.text-center {
  text-align: center;
}
.bold {
  font-weight: 600;
}
.italic {
  font-style: italic;
}
.text-normal {
  font-weight: normal;
}
.text-none-underline {
  text-decoration: none;
}
.cursor-pointer {
  cursor: pointer;
}

/* Colors */
.text-blue {
  color: #2563eb;
}
.text-black {
  color: #111827;
}
.gray-text {
  color: #6b7280;
}
.text-gray-dark {
  color: #4b5563;
}
.text-yellow-dark {
  color: #b45309;
}
.text-green {
  color: #16a34a;
}
.bg-gray {
  background-color: #f9fafb;
}
.border-blue {
  border-color: #2563eb !important;
}

/* Margins */
.mr-4 {
  margin-right: 4px;
}
.mr-8 {
  margin-right: 8px;
}
.ml-4 {
  margin-left: 4px;
}
.ml-8 {
  margin-left: 8px;
}
.ml-12 {
  margin-left: 12px;
}
.ml-16 {
  margin-left: 16px;
}
.mt-8 {
  margin-top: 8px;
}
.mt-12 {
  margin-top: 12px;
}
.mt-16 {
  margin-top: 16px;
}
.mt-20 {
  margin-top: 20px;
}
.mt-24 {
  margin-top: 24px;
}
.mt-32 {
  margin-top: 32px;
}
.mt-48 {
  margin-top: 48px;
}

/* Icons */
.icon-mr {
  margin-right: 8px;
  flex-shrink: 0;
}
.icon-blue {
  color: #2563eb;
}
.icon-gray {
  color: #6b7280;
}
.icon-trash {
  color: #9ca3af;
  cursor: pointer;
  transition: color 0.2s;
}
.icon-trash:hover {
  color: #ef4444;
}
.inline-icon {
  display: inline-block;
  vertical-align: middle;
}

/* ==========================================
   TYPOGRAPHY & LINKS
   ========================================== */
h1,
h4 {
  margin: 0;
  color: #111;
}
.form-title {
  font-size: 18px;
  text-align: center;
  margin-bottom: 16px;
  font-weight: bold;
  line-height: 30px;
}
.subtitle {
  text-align: center;
  color: #6b7280;
  line-height: 1.5;
}
.link-blue {
  color: #2563eb;
  text-decoration: none;
}
.link-blue:hover {
  text-decoration: underline;
}

/* ==========================================
   ALERTS / BANNERS
   ========================================== */
.alert-banner {
  padding: 10px 16px;
  border-radius: 4px;
  display: flex;
  align-items: flex-start;
  line-height: 1.5;
  margin: 12px 0;
}
.alert-green {
  background: #ecfdf5;
  color: #047857;
  border: 1px solid #a7f3d0;
}
.alert-yellow {
  margin: 20px 0;
  background: #fffbeb;
  border: 1px solid #fde68a;
  color: #92400e;
}
.alert-yellow-light {
  background: #fffbeb;
  border: 1px solid #fef3c7;
}
.bulb {
  margin-right: 8px;
  font-size: 14px;
}

/* ==========================================
   FORM CONTROLS & GRID
   ========================================== */
.grid-layout {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}

.grid-layout.overall-info {
  display: grid;
  margin-top: 36px;
  white-space: nowrap;
}

.grid-col-4 {
  width: calc(33.333% - 8px);
}
.grid-col-6 {
  width: calc(50% - 6px);
}
.grid-col-8 {
  width: calc(66.666% - 8px);
}
.grid-col-12 {
  width: 100%;
}

.form-group {
  display: flex;
  flex-direction: row; /* đổi từ column → row */
  align-items: center;
  gap: 8px;
}
.flex-align-center {
  display: flex;
  flex-direction: row;
  align-items: center;
}
.flex-align-start {
  display: flex;
  flex-direction: row;
  align-items: flex-start;
}
.form-group label {
  width: 160px; /* cố định label */
  margin-bottom: 0;
  font-weight: 600;
  color: #374151;
}
.static-text {
  color: #111;
}

.form-control {
  height: 30px;
  padding: 0 8px;
  border: 1px solid #d1d5db;
  border-radius: 3px;
  font-size: 13px;
  outline: none;
  width: 100%;
  font-family: inherit;
  transition: border-color 0.2s;
  flex: 1;
}
.form-control:focus {
  border-color: #2563eb;
}

/* MST Split Boxes */
.mst-boxes {
  display: flex;
  gap: 4px;
}
.mst-box {
  width: 30px;
  height: 30px;
  border: 1px solid #d1d5db;
  border-radius: 3px;
  font-weight: bold;
  background: #f9fafb;
}

/* ==========================================
   CHECKBOXES & RADIOS
   ========================================== */
.radio-group-center {
  display: flex;
  justify-content: center;
  gap: 32px;
  margin-bottom: 12px;
}
.radio-label {
  display: flex;
  align-items: center;
  gap: 6px;
  cursor: pointer;
  font-weight: 500;
}

/* Custom Checkbox giống MISA */
.checkbox-wrapper {
  display: flex;
  align-items: center;
  cursor: pointer;
  position: relative;
  user-select: none;
  margin-bottom: 10px;
  color: #374151;
}
.checkbox-wrapper input {
  position: absolute;
  opacity: 0;
  cursor: pointer;
  height: 0;
  width: 0;
}
.checkmark {
  height: 16px;
  width: 16px;
  background-color: #fff;
  border: 1px solid #d1d5db;
  border-radius: 3px;
  margin-right: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}
.checkbox-wrapper:hover input ~ .checkmark {
  border-color: #2563eb;
}
.checkbox-wrapper input:checked ~ .checkmark {
  background-color: #2563eb;
  border-color: #2563eb;
}
.checkbox-wrapper input:checked ~ .checkmark:after {
  content: '';
  width: 4px;
  height: 8px;
  border: solid white;
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
  margin-bottom: 2px;
}

.nested-options {
  margin-left: 20px;
}
.indent-options {
  margin-left: 26px;
  margin-top: 8px;
  margin-bottom: 16px;
}
.grid-checkboxes {
  display: flex;
  flex-direction: column;
  gap: 4px;
  margin-top: 12px;
}

/* ==========================================
   TABLES
   ========================================== */
.table-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.misa-table {
  width: 100%;
  border-collapse: collapse;
}
.misa-table th {
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  padding: 8px;
  font-weight: 600;
  text-align: left;
}
.misa-table td {
  border: 1px solid #e5e7eb;
  padding: 4px 8px;
  height: 36px;
  vertical-align: middle;
}
.table-input {
  width: 100%;
  border: none;
  outline: none;
  background: transparent;
  font-size: 13px;
  font-family: inherit;
}
.table-input:focus {
  border-bottom: 1px solid #2563eb;
}
.action-cells {
  display: flex;
  justify-content: center;
  align-items: center;
  border-top: none !important;
  border-left: none !important;
}

/* ==========================================
   BUTTONS
   ========================================== */
button {
  font-family: inherit;
  font-size: 13px;
}

.table-actions {
  display: flex;
  align-items: center;
}
.btn-outline {
  display: flex;
  align-items: center;
  justify-content: center;
  background: #fff;
  border: 1px solid #d1d5db;
  padding: 6px 12px;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 600;
  color: #374151;
}
.btn-outline:hover {
  background: #f9fafb;
}

.btn-sign {
  display: flex;
  align-items: center;
  justify-content: center;
  background: #fff;
  border: 1px solid #d1d5db;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
}
.btn-sign:hover {
  background: #f3f4f6;
}

/* Footer Buttons */
.footer-left,
.footer-right {
  display: flex;
  align-items: center;
  gap: 12px;
}
.btn-text {
  display: flex;
  align-items: center;
  background: none;
  border: none;
  color: #111;
  font-weight: 600;
  cursor: pointer;
  padding: 6px 8px;
  border-radius: 4px;
}
.btn-text:hover {
  background: #f3f4f6;
}
.btn-cancel {
  background: #fff;
  border: 1px solid #d1d5db;
  padding: 8px 16px;
  border-radius: 4px;
  font-weight: 600;
  cursor: pointer;
}
.btn-cancel:hover {
  background: #f9fafb;
}
.btn-primary {
  background: #2563eb;
  color: #fff;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  font-weight: 600;
  cursor: pointer;
}
.btn-primary:hover {
  background: #1d4ed8;
}

/* ==========================================
   SCROLLBAR
   ========================================== */
.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: #d1d5db;
  border-radius: 4px;
}
.mst-boxes {
  display: flex;
  gap: 6px;
}

.mst-box {
  width: 28px;
  height: 28px;
  border: 1px solid #d1d5db;
  border-radius: 2px;
  font-weight: 600;
  background: #fff;
}
.form-group.flex-align-center label {
  /* width: 200px; */
}
.signature-layout {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-top: 16px;
}
.signature-layout .sig-left input {
  width: 100px !important;
}

.sig-left {
  width: 30%;
}

.sig-right {
  width: 50%;
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;
}
.representative-info label {
  width: 140px;
}

.w-60 {
  width: 60% !important;
}
/* label */
.representative-info label {
  white-space: nowrap;
  font-weight: 600;
}

/* input */
.representative-info .form-control {
  width: 100%;
}

/* full row (2 dòng đầu) */
.representative-info .full {
  grid-column: 1 / -1;
  display: grid;
  grid-template-columns: 400px 1fr;
  gap: 16px;
  align-items: center;
}
.representative-info {
  display: grid;
  grid-template-columns: 180px 1fr 180px 1fr;
  gap: 12px 16px;
}
.label-address-nnt {
  display: flex;
  gap: 10px;
  white-space: nowrap;
  width: 180px !important;
  align-items: center;
}
.input-address-nnt {
  width: 215px !important;
}
.nnt-info .grid-col-8 {
  width: auto !important;
}
/* Khung chứa chữ ký */
.signature-valid-box {
  position: relative;
  background-color: #eaf4eb; /* Màu nền xanh lá nhạt */
  border: 1px solid #4caf50; /* Viền xanh lá */
  padding: 12px 16px;
  width: max-content;
  min-width: 260px;
  font-family: 'Times New Roman', Times, serif; /* Font chuẩn văn bản pháp lý */
  color: #cc0000; /* Chữ màu đỏ */
  overflow: hidden;
  user-select: none;
}

/* Đảm bảo nội dung nổi lên trên watermark */
.signature-content {
  position: relative;
  z-index: 2;
}

/* Tiêu đề Signature Valid */
.sig-title {
  font-size: 18px;
  font-weight: bold;
  margin-bottom: 6px;
}

/* Từng dòng thông tin */
.sig-row {
  font-size: 16px;
  margin-bottom: 4px;
}

.sig-row i {
  font-style: italic;
  font-weight: normal;
  margin-right: 4px;
}

.sig-row strong {
  font-weight: bold;
}

/* Watermark Dấu tích xanh */
.watermark-bg {
  position: absolute;
  top: 50%;
  left: 60%;
  transform: translate(-50%, -50%);
  z-index: 1;
  opacity: 0.4; /* Làm mờ để không cản trở việc đọc chữ */
}

.check-icon {
  width: 80px;
  height: 80px;
  color: #16a34a; /* Xanh lá đậm */
  filter: drop-shadow(2px 2px 2px rgba(0, 0, 0, 0.3)); /* Đổ bóng nhẹ tạo cảm giác 3D giống ảnh */
}

/* Chữ hiển thị khi chưa ký (Bạn có thể tự style lại dạng nút bấm nếu muốn) */
.unsigned-text {
  font-family: 'Inter', sans-serif; /* Trả lại font hệ thống cho phần chưa ký */
  color: #374151;
}
</style>
