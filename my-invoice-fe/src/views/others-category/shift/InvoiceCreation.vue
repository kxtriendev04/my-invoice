<script setup>
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useForm } from 'vee-validate'
import * as yup from 'yup'
import {
  Info,
  Settings,
  Trash2,
  Plus,
  MessageSquare,
  Globe,
  ChevronDown,
  ChevronUp,
  Check,
  Eye,
  Mail,
  Edit2,
} from 'lucide-vue-next'
import RibbonText from '@/components/ui/RibbonText.vue'
import invoiceApi from '@/api/invoiceApi'
import invoiceTemplateApi from '@/api/invoiceTemplateApi'
import invoicesApi from '@/api/invoicesApi'
import { mockCertificates } from '@/data/mockCertificates'
import { useRouter } from 'vue-router'

const router = useRouter()
const route = useRoute()

// Trạng thái xử lý ký số
const isPublishing = ref(false)
//#region 1. MOCK DATA
const donViTree = [
  {
    id: 1,
    label: 'Công ty cổ phần Đại Việt',
    level: 0,
    children: [
      { id: 2, label: 'Chi nhánh Hà Nội', level: 1 },
      { id: 3, label: 'Chi nhánh TP.HCM', level: 1 },
    ],
  },
]

const customers = [
  {
    id: 'KH01',
    mst: '0102030405',
    ten: 'Công ty TNHH Phần mềm ABC',
    diaChi: 'Số 1, đường A, Hà Nội',
    email: 'contact@abc.vn',
    sdt: '0987654321',
  },
  {
    id: 'KH02',
    mst: '0312345678',
    ten: 'Nguyễn Văn Nam',
    diaChi: 'Quận 1, TP.HCM',
    email: 'namnv@gmail.com',
    sdt: '0911222333',
  },
]

const inventoryItems = [
  { ma: '20026', ten: 'Vàng 519', dvt: 'Chỉ', donGia: 5190000 },
  { ma: '61200K12V00ZF', ten: 'BỘ CHẮN BÙN TRƯỚC *R368*', dvt: 'Bộ', donGia: 150000 },
  { ma: '61200K12V20ZA', ten: 'BỘ CHẮN BÙN TRƯỚC *NHB25M*', dvt: 'Bộ', donGia: 155000 },
  { ma: 'IP15PRM', ten: 'iPhone 15 Pro Max 256GB', dvt: 'Chiếc', donGia: 29900000 },
]

const thueOptions = [
  { label: '10%', value: 10 },
  { label: '8%', value: 8 },
  { label: '5%', value: 5 },
  { label: '0%', value: 0 },
  { label: 'KCT', value: -1 },
]
//#endregion

// danh sách ký hiệu / mẫu (load từ BE)
const templates = ref([])
const selectedTemplateCode = ref('')

// Các tùy chọn ký hiệu (series) load từ BE
const seriesOptions = ref([])
const selectedSeries = ref('')

// helper to read either camelCase or PascalCase keys
const getVal = (obj, ...keys) => {
  for (const k of keys) {
    if (obj && obj[k] !== undefined && obj[k] !== null) return obj[k]
  }
  return ''
}

const loadTemplates = async () => {
  try {
    // ưu tiên lấy danh sách theo công ty (DB)
    const res = await invoiceTemplateApi.getByCompany()
    // axiosClient interceptor trả về response.data (BaseResponse)
    const data = res?.data ?? res
    if (Array.isArray(data) && data.length > 0) {
      templates.value = data
    } else if (res && res.data && Array.isArray(res.data)) {
      templates.value = res.data
    } else {
      // fallback lấy mẫu default
      const def = await invoiceTemplateApi.getDefaults()
      const defData = def?.data ?? def
      templates.value = Array.isArray(defData) ? defData : []
    }

    if (templates.value.length > 0)
      selectedTemplateCode.value = templates.value[0].templateCode || ''

    // build series options from templates (field may be invSeries or InvSeries depending on serializer)
    try {
      const set = new Set()
      templates.value.forEach((t) => {
        const s = t.invSeries ?? t.InvSeries ?? t.Invseries ?? t.Invseries
        if (s) set.add(s)
      })
      seriesOptions.value = Array.from(set)
      if (seriesOptions.value.length > 0 && !selectedSeries.value)
        selectedSeries.value = seriesOptions.value[0]
    } catch (e) {
      console.warn('Không thể build series từ templates:', e)
    }
  } catch (err) {
    console.error('Không thể load ký hiệu/mẫu hóa đơn:', err)
  }
}

//#region 3. FORM & VEE-VALIDATE (moved up so setFieldValue is available before loading invoice)
const schema = yup.object({
  buyerTaxCode: yup.string().required('MST không được để trống'),
  buyerName: yup.string().required('Tên đơn vị không được trống'),
  buyerAddress: yup.string().required('Địa chỉ không được trống'),
  buyerLegalName: yup.string().nullable(),
  buyerEmail: yup.string().email('Email không hợp lệ').nullable(),
  buyerPhone: yup.string().nullable(),
  paymentMethod: yup.string().default('TM/CK'),
  buyerBankAccount: yup.string().nullable(),
  buyerBankName: yup.string().nullable(),
})

const { handleSubmit, defineField, setFieldValue, resetForm, errors } = useForm({
  validationSchema: schema,
  initialValues: { paymentMethod: 'TM/CK' },
})

const [buyerTaxCode] = defineField('buyerTaxCode')
const [msdvcq] = defineField('msdvcq')
const [maDonVi] = defineField('maDonVi')
const [buyerName] = defineField('buyerName')
const [buyerAddress] = defineField('buyerAddress')
const [buyerLegalName] = defineField('buyerLegalName')
const [buyerEmail] = defineField('buyerEmail')
const [buyerPhone] = defineField('buyerPhone')
const [paymentMethod] = defineField('paymentMethod')
const [buyerBankAccount] = defineField('buyerBankAccount')
const [buyerBankName] = defineField('buyerBankName')

const selectCustomer = (cust) => {
  setFieldValue('buyerTaxCode', cust.mst)
  setFieldValue('buyerName', cust.ten)
  setFieldValue('buyerAddress', cust.diaChi)
  setFieldValue('buyerLegalName', cust.ten)
  setFieldValue('buyerEmail', cust.email)
  setFieldValue('buyerPhone', cust.sdt)
}
//#endregion

const isEditMode = ref(false)
const isViewMode = ref(false)
const currentInvoiceId = ref(null)

const loadInvoiceForEdit = async (id) => {
  try {
    const res = await invoiceApi.getById(id)
    const payload = res?.data ?? res
    if (!payload) return

    // If backend wraps in BaseResponse
    const data = payload.data ?? payload

    // mark edit mode
    isEditMode.value = true
    currentInvoiceId.value = id

    // debug
    console.debug('Loaded invoice payload:', payload)

    // If issued (issueStatus === 1) then view-only
    // const issueStatus = getVal(data, 'issueStatus', 'IssueStatus')
    // if (issueStatus === 1) {
    //   isViewMode.value = true
    // }

    // Map header
    try {
      // Theo response mới, dữ liệu chung nằm trong đối tượng invoice
      const inv = data.invoice ?? data

      // Update vee-validate form store
      resetForm({
        values: {
          buyerTaxCode: inv.buyerTaxCode || '',
          buyerName: inv.buyerName || '',
          buyerAddress: inv.buyerAddress || '',
          buyerEmail: inv.buyerEmail || '',
          buyerPhone: inv.buyerPhone || '',
          buyerLegalName: inv.buyerLegalName || inv.buyerName || '',
          paymentMethod: inv.paymentMethod || 'TM/CK',
          buyerBankAccount: inv.buyerBankAccount || '',
          buyerBankName: inv.buyerBankName || '',
        },
      })

      selectedTemplateCode.value = getVal(inv, 'templateCode', 'TemplateCode')
      selectedSeries.value = getVal(inv, 'series', 'Series')

      // Map details if provided (supports both 'details' and 'Details')
      const detailsArr = data.details ?? data.Details
      if (Array.isArray(detailsArr) && detailsArr.length > 0) {
        invoiceDetails.value = detailsArr.map((d) => ({
          detailId: d.detailId ?? Date.now() + Math.random(),
          tinhChat: 'Hàng hóa, dịch vụ',
          itemCode: d.itemCode || '',
          itemName: d.itemName || '',
          unitName: d.unitName || '',
          quantity: d.quantity ?? 1,
          unitPrice: d.unitPrice ?? 0,
          amount: d.amount ?? 0,
        }))
      } else {
        // Nếu không có dữ liệu hàng hóa từ server, khởi tạo 1 dòng trống để người dùng nhập
        invoiceDetails.value = [
          {
            detailId: Date.now(),
            tinhChat: 'Hàng hóa, dịch vụ',
            itemCode: '',
            itemName: '',
            unitName: '',
            quantity: 1,
            unitPrice: 0,
            amount: 0,
          },
        ]
      }
    } catch (e) {
      console.warn('Mapping invoice payload failed', e)
    }
  } catch (err) {
    console.error('Không thể load hóa đơn để sửa:', err)
    alert('Không thể tải hóa đơn để sửa.')
  }
}

//#region 2. QUẢN LÝ DROPDOWNS CUSTOM
const showDonViDropdown = ref(false)
const selectedDonVi = ref(donViTree[0].label)
const showCustomerDropdown = ref(false)
const showItemDropdownIndex = ref(-1)

const closeAllDropdowns = () => {
  showDonViDropdown.value = false
  showCustomerDropdown.value = false
  showItemDropdownIndex.value = -1
}
onMounted(() => document.addEventListener('click', closeAllDropdowns))
onUnmounted(() => document.removeEventListener('click', closeAllDropdowns))
onMounted(() => {
  // load ký hiệu / mẫu khi mount
  loadTemplates()
  const invId = route.params.id
  if (invId && invId !== 'create') {
    loadInvoiceForEdit(invId)
  }
})

// Nếu templates thay đổi, đảm bảo seriesOptions cập nhật (redundant với nội dung trong loadTemplates nhưng an toàn)
watch(templates, (n) => {
  try {
    const set = new Set()
    n.forEach((t) => {
      const s = t.invSeries ?? t.InvSeries ?? t.Invseries ?? t.Invseries
      if (s) set.add(s)
    })
    seriesOptions.value = Array.from(set)
    if (seriesOptions.value.length > 0 && !selectedSeries.value)
      selectedSeries.value = seriesOptions.value[0]
  } catch (e) {
    // ignore
  }
})
//#endregion

//#region 4. QUẢN LÝ BẢNG HÀNG HÓA
const invoiceDetails = ref([
  {
    detailId: Date.now(),
    tinhChat: 'Hàng hóa, dịch vụ',
    itemCode: '',
    itemName: '',
    unitName: '',
    quantity: 1,
    unitPrice: 0,
    amount: 0,
  },
])

const addRow = () => {
  invoiceDetails.value.push({
    detailId: Date.now(),
    tinhChat: 'Hàng hóa, dịch vụ',
    itemCode: '',
    itemName: '',
    unitName: '',
    quantity: 1,
    unitPrice: 0,
    amount: 0,
  })
}

const removeRow = (index) => {
  if (invoiceDetails.value.length > 1) {
    invoiceDetails.value.splice(index, 1)
  }
}

const selectItem = (index, item) => {
  invoiceDetails.value[index].itemCode = item.ma
  invoiceDetails.value[index].itemName = item.ten
  invoiceDetails.value[index].unitName = item.dvt
  invoiceDetails.value[index].unitPrice = item.donGia
  calculateRow(index)
}

const calculateRow = (index) => {
  const row = invoiceDetails.value[index]
  row.amount = (row.quantity || 0) * (row.unitPrice || 0)
}

const thueSuat = ref(10)
const tongTienHang = computed(() =>
  invoiceDetails.value.reduce((sum, item) => sum + (item.amount || 0), 0),
)
const tienThueGTGT = computed(() => {
  if (thueSuat.value === -1) return 0
  return (tongTienHang.value * thueSuat.value) / 100
})
const tongThanhToan = computed(() => tongTienHang.value + tienThueGTGT.value)

const formatCurrency = (val) => new Intl.NumberFormat('vi-VN').format(val || 0)
//#endregion

//#region 5. SUBMIT FORM
/**
 * Hàm xử lý Ký và Phát hành hóa đơn
 */
const handlePublish = async () => {
  try {
    // 1. Kiểm tra và Lưu dữ liệu hóa đơn trước khi phát hành
    // Nếu chưa có ID hoặc đang sửa, ta thực hiện lưu để đảm bảo dữ liệu mới nhất được ký
    let invoiceId = currentInvoiceId.value

    if (!invoiceId) {
      // Nếu chưa lưu nháp, thực hiện lưu trước
      // ✅ đúng
      const values = {
        buyerTaxCode: buyerTaxCode.value,
        buyerName: buyerName.value,
        buyerAddress: buyerAddress.value,
        buyerEmail: buyerEmail.value,
        buyerPhone: buyerPhone.value,
        buyerLegalName: buyerLegalName.value,
        paymentMethod: paymentMethod.value,
        buyerBankAccount: buyerBankAccount.value,
        buyerBankName: buyerBankName.value,
      }
      invoiceId = await processSubmit(values, true)
      if (!invoiceId) return
    }

    isPublishing.value = true

    // 2. Lấy nội dung XML chưa ký từ server (giả sử endpoint này tồn tại)
    // const xmlRes = await invoiceApi.getUnsignedXml(invoiceId);
    // const unsignedXml = xmlRes.data;
    const unsignedXml = `<HDon><DLHDon><Id>${invoiceId}</Id><TenHDon>Hóa đơn GTGT</TenHDon></DLHDon></HDon>`

    // 3. Gọi Tool ký số (Giả lập tương tác với MISA Tool qua Localhost hoặc WebSocket)
    console.log('Đang gọi Tool ký số với XML:', unsignedXml)
    const signedXml = await callMisaSignTool(unsignedXml)
    debugger
    // 4. Gửi XML đã có chữ ký lên Backend để lưu trữ và cấp số
    const res = await invoicesApi.signAndPublish(invoiceId, {
      signedXmlContent: signedXml,
    })

    if (res) {
      alert('Hóa đơn đã được ký và gửi cho cơ quan thuế thành công!')
      router.push('/invoice/list')
    }
  } catch (err) {
    console.error('Lỗi phát hành:', err)
    alert('Phát hành thất bại: ' + (err.response?.data?.userMsg || err.message))
  } finally {
    isPublishing.value = false
  }
}

/**
 * Giả lập gọi Tool MISA ký số
 * Trong thực tế, bạn sẽ dùng fetch() gọi đến service của MISA chạy ở localhost
 */
const callMisaSignTool = async (xml) => {
  return new Promise((resolve, reject) => {
    setTimeout(() => {
      const certListStr = mockCertificates.map((c) => `${c.id}: ${c.subject}`).join('\n')
      const selectedId = window.prompt(`CHỌN CHỨNG THƯ SỐ ĐỂ KÝ HÓA ĐƠN:\n${certListStr}`, '1')

      const cert = mockCertificates.find((c) => c.id === selectedId)

      if (cert) {
        const fakeSignature = `
          <Signature xmlns="http://www.w3.org/2000/09/xmldsig#">
            <SignedInfo><SignatureMethod Algorithm="rsa-sha1"/><Reference><DigestValue>FAKE_DIGEST</DigestValue></Reference></SignedInfo>
            <SignatureValue>FAKE_VALUE_${cert.serial}</SignatureValue>
            <KeyInfo><X509Data><X509Certificate>MOCK_CERT_OF_${cert.subject}</X509Certificate></X509Data></KeyInfo>
          </Signature>`

        const completedXml = xml.replace('</HDon>', fakeSignature + '</HDon>')
        resolve(completedXml)
      } else {
        reject(new Error('Người dùng đã hủy hoặc không chọn chứng thư số.'))
      }
    }, 1000)
  })
}

const onSave = handleSubmit(async (values) => {
  await processSubmit(values, false)
})

const processSubmit = async (values, isPublish) => {
  // Map frontend form to backend InvoiceCreateDto expected shape
  const invoicePayload = {
    invoice: {
      // basic mapping - expand as needed
      sellerName: 'Công ty cổ phần Đại Việt',
      sellerTaxCode: '0101243150-996',
      sellerAddress: 'Tầng 9 Technosoft, Phường Cầu Giấy, Thành phố Hà Nội, Việt Nam',
      buyerName: values.buyerName || '',
      buyerTaxCode: values.buyerTaxCode || '',
      buyerAddress: values.buyerAddress || '',
      buyerEmail: values.buyerEmail || '',
      buyerPhone: values.buyerPhone || '',
      buyerLegalName: values.buyerLegalName || '',
      paymentMethod: values.paymentMethod || 'TM/CK',
      templateCode: selectedTemplateCode.value || null,
      series: selectedSeries.value,
      invoiceDate: new Date().toISOString(),
      totalBeforeTax: tongTienHang.value,
      totalTaxAmount: tienThueGTGT.value,
      totalAmount: tongThanhToan.value,
    },
    details: invoiceDetails.value.map((d) => ({
      itemCode: d.itemCode,
      itemName: d.itemName,
      unitName: d.unitName,
      quantity: d.quantity,
      unitPrice: d.unitPrice,
      taxRate: thueSuat.value === -1 ? 0 : thueSuat.value,
      amount: d.amount,
    })),
  }

  try {
    if (!selectedSeries.value) {
      alert('Vui lòng chọn Ký hiệu (Series)')
      return
    }
    let res
    if (currentInvoiceId.value && !isViewMode.value) {
      // Update existing invoice
      res = await invoiceApi.update(currentInvoiceId.value, invoicePayload)
      console.log('Update invoice response:', res)
      alert('Cập nhật hóa đơn thành công')
    } else {
      res = await invoiceApi.create(invoicePayload)
      console.log('Create invoice response:', res)
      alert('Lập hóa đơn thành công')
      if (res.data) currentInvoiceId.value = res.data
    }
    if (!isPublish) router.push('/invoice/list')
    return res.data
  } catch (err) {
    console.error('Lỗi khi gửi yêu cầu tạo hóa đơn', err)
    alert('Có lỗi khi tạo hóa đơn. Xem console để biết chi tiết.')
  }
}
//#endregion

const isThanhToan = ref(false)
const isQuyetToan = ref(false)
const hasKemBangKe = ref(false)
</script>

<template>
  <div class="misa-invoice-page">
    <div class="page-content custom-scrollbar">
      <div class="page-header">
        <h2 class="title">Lập hóa đơn</h2>

        <div class="header-actions">
          <div
            class="custom-dropdown-container"
            @click.stop="showDonViDropdown = !showDonViDropdown"
          >
            <div class="dropdown-trigger">
              <span class="truncate">{{ selectedDonVi }}</span>
              <ChevronDown :size="14" class="icon-gray" />
            </div>
            <div v-if="showDonViDropdown" class="dropdown-menu tree-menu">
              <ul>
                <li v-for="node in donViTree" :key="node.id">
                  <div class="tree-node parent-node" @click="selectedDonVi = node.label">
                    <Check v-if="selectedDonVi === node.label" :size="14" class="icon-check" />
                    {{ node.label }}
                  </div>
                  <ul class="tree-sub-menu">
                    <li
                      v-for="child in node.children"
                      :key="child.id"
                      class="tree-node"
                      @click="selectedDonVi = child.label"
                    >
                      <Check v-if="selectedDonVi === child.label" :size="14" class="icon-check" />
                      {{ child.label }}
                    </li>
                  </ul>
                </li>
              </ul>
            </div>
          </div>

          <label class="checkbox-wrapper">
            <input type="checkbox" v-model="isThanhToan" /> <span class="checkmark"></span> Đã thanh
            toán
          </label>
          <label class="checkbox-wrapper">
            <input type="checkbox" v-model="isQuyetToan" /> <span class="checkmark"></span> Là hóa
            đơn quyết toán
            <Info :size="14" class="icon-blue ml-4" />
          </label>
        </div>
      </div>

      <div class="paper-container">
        <RibbonText
          icon="💡"
          text="Bạn không biết cách tạo hoá đơn"
          actionText="Xem hướng dẫn"
          color="orange"
          url="https://www.youtube.com/watch?v=dQw4w9WgXcQ"
        />

        <div class="invoice-title">
          <h1>HÓA ĐƠN GIÁ TRỊ GIA TĂNG</h1>
          <p>Ngày 12 tháng 03 năm 2026</p>
        </div>

        <div class="top-info">
          <div class="seller-info">
            <div class="info-row">
              <div class="lbl">Đơn vị bán hàng:</div>
              <div class="val">Công ty cổ phần Đại Việt</div>
            </div>
            <div class="info-row">
              <div class="lbl">Mã số thuế:</div>
              <div class="val">0101243150-996</div>
            </div>
            <div class="info-row">
              <div class="lbl">Địa chỉ:</div>
              <div class="val">Tầng 9 Technosoft, Phường Cầu Giấy, Thành phố Hà Nội, Việt Nam</div>
            </div>
          </div>

          <div class="kyhieu-box">
            <div class="kh-row">
              <span class="kh-lbl">Ký hiệu:</span>
              <div style="flex: 1; display: flex; gap: 8px; align-items: center">
                <select v-model="selectedSeries" class="kh-select">
                  <option v-for="s in seriesOptions" :key="s" :value="s">{{ s }}</option>
                </select>
              </div>
            </div>
            <div class="divider-dash"></div>
            <div class="kh-row">
              <span class="kh-lbl">Số:</span>
              <span class="kh-empty">&lt;Chưa cấp số&gt;</span>
              <Info :size="14" class="icon-blue" />
            </div>
          </div>
        </div>

        <form id="invoiceForm" @submit="onSaveAndIssue" class="buyer-form">
          <div class="f-row">
            <div class="f-label">MST/CCCD chủ hộ:</div>
            <div class="f-control group-control">
              <div class="input-wrapper w-33" @click.stop="showCustomerDropdown = true">
                <input
                  v-model="buyerTaxCode"
                  type="text"
                  class="misa-dot-input w-100"
                  :class="{ error: errors.buyerTaxCode }"
                  placeholder="Nhập MST..."
                />
                <ChevronDown class="icon-absolute" :size="14" />

                <div v-if="showCustomerDropdown" class="dropdown-menu mega-menu w-500">
                  <table class="grid-table">
                    <thead>
                      <tr>
                        <th>Mã số thuế</th>
                        <th>Tên khách hàng</th>
                        <th>Địa chỉ</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="c in customers" :key="c.id" @click="selectCustomer(c)">
                        <td>{{ c.mst }}</td>
                        <td>{{ c.ten }}</td>
                        <td>{{ c.diaChi }}</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
              <button type="button" class="btn-link"><Globe :size="14" /> Lấy thông tin</button>
              <button type="button" class="btn-link">
                <Info :size="14" /> KT tình trạng hoạt động của KH
              </button>
            </div>
          </div>

          <div class="f-row">
            <div class="f-label">Tên đơn vị:</div>
            <div class="f-control">
              <input
                v-model="buyerName"
                type="text"
                class="misa-dot-input w-100"
                :class="{ error: errors.buyerName }"
              />
            </div>
          </div>

          <div class="f-row">
            <div class="f-label">Địa chỉ:</div>
            <div class="f-control group-control">
              <input
                v-model="buyerAddress"
                type="text"
                class="misa-dot-input flex-1"
                :class="{ error: errors.buyerAddress }"
              />
              <Settings :size="16" class="icon-gray cursor-pointer" />
            </div>
          </div>

          <div class="f-row">
            <div class="f-label">Người mua hàng:</div>
            <div class="f-control">
              <input v-model="buyerLegalName" type="text" class="misa-dot-input w-100" />
            </div>
          </div>

          <div class="f-row split">
            <div class="f-col">
              <div class="f-label">Email:</div>
              <div class="f-control">
                <input
                  v-model="buyerEmail"
                  type="text"
                  class="misa-dot-input w-100"
                  :class="{ error: errors.buyerEmail }"
                />
              </div>
            </div>
            <div class="f-col">
              <div class="f-label text-right pr-16">Số điện thoại:</div>
              <div class="f-control">
                <input v-model="buyerPhone" type="text" class="misa-dot-input w-100" />
              </div>
            </div>
          </div>

          <div class="f-row">
            <div class="f-label">Hình thức TT:</div>
            <div class="f-control input-wrapper w-33">
              <select v-model="paymentMethod" class="misa-dot-input w-100 no-arrow cursor-pointer">
                <option value="TM/CK">TM/CK</option>
                <option value="Tiền mặt">Tiền mặt</option>
                <option value="Chuyển khoản">Chuyển khoản</option>
              </select>
              <ChevronDown class="icon-absolute" :size="14" />
            </div>
          </div>

          <div class="f-row split">
            <div class="f-col">
              <div class="f-label">TK ngân hàng:</div>
              <div class="f-control">
                <input v-model="buyerBankAccount" type="text" class="misa-dot-input w-100" />
              </div>
            </div>
            <div class="f-col">
              <div class="f-label text-right pr-16">Tên ngân hàng:</div>
              <div class="f-control">
                <input v-model="buyerBankName" type="text" class="misa-dot-input w-100" />
              </div>
            </div>
          </div>
        </form>

        <div class="goods-section">
          <div class="goods-toolbar-top">
            <label class="checkbox-wrapper">
              <input type="checkbox" v-model="hasKemBangKe" /> <span class="checkmark"></span> Lập
              kèm bảng kê hàng hóa, dịch vụ.
            </label>
            <a href="#" class="link-blue ml-8">Xem hướng dẫn</a>
          </div>

          <div class="goods-toolbar-main">
            <div class="toolbar-left">
              <span class="bold">Hàng hóa/Dịch vụ</span>
              <label class="checkbox-wrapper ml-16"
                ><input type="checkbox" checked /> <span class="checkmark"></span> Hiện cột "Tính
                chất HHDV"</label
              >
            </div>
            <div class="toolbar-right">
              <div class="tool-item">
                Loại tiền:
                <select class="tool-select">
                  <option>VND</option>
                </select>
              </div>
              <div class="tool-item">
                Tỷ giá: <input type="text" value="1" class="tool-input w-40 text-right" />
              </div>
              <div class="tool-item">
                Chiết khấu:
                <select class="tool-select">
                  <option>Không có chiết khấu</option>
                </select>
              </div>
            </div>
          </div>

          <table class="misa-table">
            <thead>
              <tr>
                <th width="40" class="text-center">STT</th>
                <th width="150">
                  Tính chất HHDV <Info :size="12" class="icon-blue inline-icon" />
                </th>
                <th width="150">Mã hàng hóa</th>
                <th>Tên hàng hóa/Dịch vụ</th>
                <th width="70">ĐVT</th>
                <th width="80" class="text-right">Số lượng</th>
                <th width="110" class="text-right">Đơn giá sau thuế</th>
                <th width="130" class="text-right">Thành tiền sau thuế</th>
                <th width="40" class="text-center"></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, idx) in invoiceDetails" :key="item.detailId">
                <td class="text-center gray-text">{{ idx + 1 }}</td>
                <td>
                  <div class="input-wrapper">
                    <select v-model="item.tinhChat" class="table-input no-arrow">
                      <option>Hàng hóa, dịch vụ</option>
                      <option>Chiết khấu thương mại</option>
                      <option>Ghi chú</option>
                    </select>
                    <ChevronDown class="icon-absolute" :size="14" />
                  </div>
                </td>
                <td class="input-wrapper" @click.stop="showItemDropdownIndex = idx">
                  <input
                    v-model="item.itemCode"
                    type="text"
                    class="table-input"
                    placeholder="Tìm mã..."
                  />
                  <ChevronDown class="icon-absolute" :size="14" />

                  <div
                    v-if="showItemDropdownIndex === idx"
                    class="dropdown-menu mega-menu w-500 fix-dropdown-pos"
                  >
                    <table class="grid-table">
                      <thead>
                        <tr>
                          <th width="120">Mã hàng hóa</th>
                          <th>Tên hàng hóa</th>
                          <th width="100">Mô tả</th>
                        </tr>
                      </thead>
                      <tbody>
                        <tr
                          v-for="inv in inventoryItems"
                          :key="inv.ma"
                          @click="selectItem(idx, inv)"
                        >
                          <td class="bold text-blue">{{ inv.ma }}</td>
                          <td>{{ inv.ten }}</td>
                          <td></td>
                        </tr>
                      </tbody>
                    </table>
                    <div class="add-new-row">
                      <Plus :size="14" class="inline-icon" /> Thêm mới (F9)
                    </div>
                  </div>
                </td>
                <td><input v-model="item.itemName" type="text" class="table-input" /></td>
                <td><input v-model="item.unitName" type="text" class="table-input" /></td>
                <td>
                  <input
                    v-model.number="item.quantity"
                    type="number"
                    class="table-input text-right"
                    @input="calculateRow(idx)"
                  />
                </td>
                <td>
                  <input
                    v-model.number="item.unitPrice"
                    type="number"
                    class="table-input text-right"
                    @input="calculateRow(idx)"
                  />
                </td>
                <td class="text-right bg-gray">{{ formatCurrency(item.amount) }}</td>
                <td class="text-center">
                  <Trash2 :size="16" class="icon-trash" @click="removeRow(idx)" />
                </td>
              </tr>
            </tbody>
          </table>

          <div class="table-footer-actions">
            <button type="button" class="btn-outline" @click="addRow">
              <Plus :size="14" /> Thêm dòng
            </button>
            <button type="button" class="btn-outline">
              <MessageSquare :size="14" /> Thêm ghi chú
            </button>
          </div>
        </div>

        <div class="summary-section">
          <div class="summary-row">
            <div class="sum-lbl-1">Thuế GTGT:</div>
            <div class="sum-input input-wrapper">
              <select v-model="thueSuat" class="misa-dot-input w-100 no-arrow">
                <option v-for="opt in thueOptions" :key="opt.value" :value="opt.value">
                  {{ opt.label }}
                </option>
              </select>
              <ChevronDown class="icon-absolute" :size="14" />
            </div>
            <div class="sum-lbl-2">Tổng tiền hàng:</div>
            <div class="sum-val">{{ formatCurrency(tongTienHang) }}</div>
          </div>

          <div class="summary-row">
            <div class="sum-lbl-1"></div>
            <div class="sum-input"></div>
            <div class="sum-lbl-2">Tiền thuế GTGT:</div>
            <div class="sum-val">{{ formatCurrency(tienThueGTGT) }}</div>
          </div>

          <div class="summary-row">
            <div class="sum-lbl-1"></div>
            <div class="sum-input"></div>
            <div class="sum-lbl-2 bold text-black">Tổng tiền thanh toán:</div>
            <div class="sum-val text-black">{{ formatCurrency(tongThanhToan) }}</div>
          </div>
        </div>
      </div>
    </div>

    <div class="page-footer">
      <div class="footer-left">
        <button type="button" class="btn-text">
          <Settings :size="16" class="mr-4" /> Thêm trường mở rộng
        </button>
        <button type="button" class="btn-text">
          <Mail :size="16" class="mr-4" /> Gửi hóa đơn nháp
        </button>
        <button type="button" class="btn-text">
          <Eye :size="16" class="mr-4" /> Xem trước <ChevronUp :size="14" class="ml-4" />
        </button>
      </div>
      <div class="footer-right">
        <div v-if="isPublishing" class="publishing-overlay">
          <span class="loader"></span> Đang thực hiện ký số...
        </div>
        <button type="button" class="btn-cancel">Hủy</button>
        <button type="button" class="btn-cancel" @click="onSave">Lưu</button>
        <button
          type="button"
          class="btn-primary flex-align-center"
          @click="handlePublish"
          :disabled="isPublishing"
        >
          <Edit2 :size="14" class="mr-8" /> Lưu và phát hành
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* RESET & CƠ BẢN */
* {
  box-sizing: border-box;
}
.misa-invoice-page {
  display: flex;
  flex-direction: column;
  height: 100vh;
  width: 100%;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  font-size: 14px;
  color: #111827;
  background: #f0f2f4;
  min-height: 0;
}

.publishing-overlay {
  display: flex;
  align-items: center;
  gap: 10px;
  color: #2563eb;
  font-weight: 600;
}
.loader {
  border: 2px solid #f3f3f3;
  border-top: 2px solid #3498db;
  border-radius: 50%;
  width: 14px;
  height: 14px;
  animation: spin 1s linear infinite;
}
@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

/* UTILITIES PURE CSS */
.w-100 {
  width: 100%;
}
.w-33 {
  width: 33.333%;
}
.w-40 {
  width: 40px;
}
.w-500 {
  width: 500px;
}
.flex-1 {
  flex: 1;
}
.text-right {
  text-align: right;
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
.text-blue {
  color: #2563eb;
}
.text-black {
  color: #000;
}
.gray-text {
  color: #6b7280;
}
.bg-gray {
  background-color: #f9fafb;
}
.mr-4 {
  margin-right: 4px;
}
.ml-4 {
  margin-left: 4px;
}
.ml-8 {
  margin-left: 8px;
}
.ml-16 {
  margin-left: 16px;
}
.pr-16 {
  padding-right: 16px;
}
.mt-6 {
  margin-top: 24px;
}
.mt-8 {
  margin-top: 32px;
}
.inline-icon {
  display: inline-block;
  vertical-align: middle;
}

/* ICONS */
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
  margin: 0 auto;
  display: block;
}
.icon-trash:hover {
  color: #ef4444;
}
.input-wrapper {
  position: relative;
}
.icon-absolute {
  position: absolute;
  right: 4px;
  top: 50%;
  transform: translateY(-50%);
  color: #9ca3af;
  pointer-events: none;
}
.no-arrow {
  appearance: none;
  -webkit-appearance: none;
}

/* HEADER */
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 24px;
  margin: 0 20px;
}
.title {
  font-size: 20px;
  font-weight: 700;
  margin: 0;
}
.header-actions {
  display: flex;
  align-items: center;
  gap: 20px;
}

/* CHECKBOX CUSTOM */
.checkbox-wrapper {
  display: flex;
  align-items: center;
  cursor: pointer;
  position: relative;
  user-select: none;
}
.header-actions .checkbox-wrapper {
  background-color: #ffffff;
  padding: 6px 12px;
  border-radius: 4px;
  border: 1px solid #d1d5db;
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

/* DROPDOWN MENU */
.custom-dropdown-container {
  position: relative;
  cursor: pointer;
}
.dropdown-trigger {
  border: 1px solid #d1d5db;
  padding: 6px 12px;
  border-radius: 4px;
  background: white;
  width: 220px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.truncate {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.dropdown-menu {
  position: absolute;
  top: calc(100% + 4px);
  left: 0;
  background: #fff;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  z-index: 50;
}
.fix-dropdown-pos {
  left: 0;
}
.tree-menu {
  min-width: 100%;
  padding: 8px 0;
}
.tree-node {
  padding: 6px 16px;
  cursor: pointer;
  display: flex;
  align-items: center;
}
.tree-node:hover {
  background: #f3f4f6;
  color: #2563eb;
}
.parent-node {
  font-weight: 600;
}
.tree-sub-menu {
  padding-left: 16px;
  margin: 0;
  list-style: none;
}
.icon-check {
  margin-right: 4px;
  color: #2563eb;
}

/* MEGA MENU GRID */
.mega-menu {
  max-height: 300px;
  overflow-y: auto;
  padding: 0;
}
.grid-table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;
}
.grid-table th {
  background: #f9fafb;
  padding: 8px 12px;
  border-bottom: 1px solid #e5e7eb;
  position: sticky;
  top: 0;
  z-index: 10;
  font-weight: 600;
}
.grid-table td {
  padding: 8px 12px;
  border-bottom: 1px solid #f3f4f6;
  cursor: pointer;
}
.grid-table tbody tr:hover td {
  background: #eff6ff;
}
.add-new-row {
  padding: 8px;
  border-top: 1px solid #e5e7eb;
  color: #2563eb;
  cursor: pointer;
  font-weight: 600;
}
.add-new-row:hover {
  background: #f9fafb;
}

/* CONTENT AREA */
.page-content {
  flex: 1;
  overflow-y: auto;
  padding: 24px;
  padding-top: 0;
  min-height: 0;
}
.paper-container {
  max-width: 900px;
  margin: 0 auto;
  background: #fff;
  padding: 24px 32px;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.05);
}

/* BANNER & TIÊU ĐỀ */
.orange-banner {
  background: #ffedd5;
  border-left: 4px solid #f97316;
  padding: 8px 16px;
  border-radius: 2px;
  display: flex;
  align-items: center;
  gap: 8px;
  color: #c2410c;
  margin-bottom: 24px;
  font-weight: 500;
}
.orange-banner a {
  color: #ea580c;
  text-decoration: underline;
  font-weight: bold;
}
.invoice-title {
  text-align: center;
  margin-bottom: 32px;
}
.invoice-title h1 {
  font-size: 22px;
  font-weight: bold;
  margin: 0 0 4px;
  letter-spacing: 0.5px;
}
.invoice-title p {
  color: #6b7280;
  font-style: italic;
  margin: 0;
}

/* TOP INFO */
.top-info {
  display: flex;
  justify-content: space-between;
  margin-bottom: 32px;
}
.seller-info {
  flex: 1;
  display: flex;
  flex-direction: column;
}
.info-row {
  display: flex;
  height: 44px;
}
.info-row .lbl {
  width: 120px;
  color: #111;
}
.info-row .val {
  font-weight: 600;
}

.kyhieu-box {
  width: 250px;
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 4px;
  padding: 12px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.kh-row {
  display: flex;
  align-items: center;
  gap: 8px;
  justify-content: space-between;
}
.kh-lbl {
  color: #111;
  width: 60px;
}
.kh-select {
  flex: 1;
  background: transparent;
  border: none;
  border-bottom: 1px dotted #9ca3af;
  outline: none;
  font-weight: 600;
  appearance: none;
  height: 30px;
  font-size: 20px;
}
.kh-empty {
  color: #9ca3af;
  font-style: italic;
  flex: 1;
}
.divider-dash {
  border-bottom: 1px dashed #d1d5db;
}

/* BUYER FORM */
.buyer-form {
  display: flex;
  flex-direction: column;
  gap: 14px;
  margin-bottom: 32px;
}
.f-row {
  display: flex;
  align-items: flex-end;
  min-height: 26px;
}
.f-label {
  width: 140px;
  padding-bottom: 4px;
  color: #111;
}
.f-control {
  flex: 1;
}
.split {
  display: flex;
  gap: 24px;
}
.f-col {
  flex: 1;
  display: flex;
  align-items: flex-end;
}
.group-control {
  display: flex;
  align-items: center;
  gap: 16px;
}

/* INPUT DOTTED */
.misa-dot-input {
  height: 26px;
  background: transparent;
  border: none;
  border-bottom: 1px dotted #9ca3af;
  outline: none;
  padding: 0 4px;
  font-family: inherit;
  font-size: 14px;
  transition: border-color 0.2s;
}
.misa-dot-input:focus {
  border-bottom: 1px solid #2563eb;
}
.misa-dot-input.error {
  border-bottom: 1px solid #ef4444;
}

.btn-link {
  background: none;
  border: none;
  color: #2563eb;
  font-weight: 500;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 4px;
  padding: 0 8px;
}
.btn-link:hover {
  text-decoration: underline;
}
.link-blue {
  color: #2563eb;
}
.link-blue:hover {
  text-decoration: underline;
}

/* BẢNG HÀNG HÓA */
.goods-toolbar-top {
  display: flex;
  align-items: center;
  margin-bottom: 12px;
}
.goods-toolbar-main {
  background: #eff6ff;
  border: 1px solid #e5e7eb;
  border-bottom: none;
  padding: 8px 12px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.toolbar-left,
.toolbar-right {
  display: flex;
  align-items: center;
}
.tool-item {
  margin-left: 16px;
  color: #374151;
}
.tool-select,
.tool-input {
  border: none;
  border-bottom: 1px solid #9ca3af;
  background: transparent;
  outline: none;
  font-family: inherit;
  font-size: 14px;
  margin-left: 4px;
  padding-bottom: 2px;
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
  font-size: 14px;
  font-family: inherit;
}

.table-footer-actions {
  display: flex;
  gap: 12px;
  margin-top: 12px;
}
.btn-outline {
  display: flex;
  align-items: center;
  gap: 6px;
  background: #fff;
  border: 1px solid #d1d5db;
  padding: 4px 12px;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
  height: 28px;
}
.btn-outline:hover {
  background: #f9fafb;
}

/* SUMMARY SECTION (GÓC PHẢI DƯỚI) */
.summary-section {
  margin-top: 32px;
  margin-left: auto;
  width: 450px;
}
.summary-row {
  display: flex;
  align-items: flex-end;
  margin-bottom: 12px;
}
.sum-lbl-1 {
  width: 100px;
  color: #111;
}
.sum-input {
  width: 80px;
}
.sum-lbl-2 {
  flex: 1;
  text-align: right;
  padding-right: 24px;
  color: #111;
}
.sum-val {
  width: 130px;
  text-align: right;
  font-weight: 700;
  border-bottom: 1px dotted #9ca3af;
  padding-bottom: 2px;
}

/* FOOTER FIXED */
.page-footer {
  background: #fafafa;
  padding: 12px 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  z-index: 10;
  height: 60px;
}
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
  color: #111827;
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

/* SCROLLBAR */
.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: #d1d5db;
  border-radius: 4px;
}
</style>
