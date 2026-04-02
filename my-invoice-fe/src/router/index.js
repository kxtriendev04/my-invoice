import MainLayout from '@/layouts/MainLayout.vue'
import LoginView from '@/views/auth/LoginView.vue'
import DashboardView from '@/views/dashboard/DashboardView.vue'
import InvoiceCreation from '@/views/others-category/shift/InvoiceCreation.vue'
import ShiftView from '@/views/others-category/shift/ShiftView.vue'
import NotFound from '@/views/NotFound.vue'
// Admin views
import CompanyList from '@/views/admin/CompanyList.vue'
import UserList from '@/views/admin/UserList.vue'
import AssignUserToCompany from '@/views/admin/AssignUserToCompany.vue'
import { createRouter, createWebHistory } from 'vue-router'
import InvoiceDeclaration from '@/views/publish-declaration/InvoiceDeclaration.vue'
import DeclarationList from '@/views/publish-declaration/DeclarationList.vue'
import TemplateList from '@/views/template/TemplateList.vue'
import TemplateSetup from '@/views/template/TemplateSetup.vue'
import TemplateEditor from '@/views/template/TemplateEditor.vue'
import TaxPortal from '@/api/TaxPortal.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  scrollBehavior() {
    return { top: 0 }
  },
  routes: [
    {
      path: '/tax-portal',
      name: 'TaxPortal',
      // Lưu ý: Đảm bảo đường dẫn file khớp với nơi bạn đã lưu component
      component: TaxPortal,
      meta: {
        layout: 'empty', // Nếu bạn không muốn trang này hiển thị Sidebar/Header của App chính
        title: 'Cổng thông tin Tổng cục Thuế (Giả lập)',
      },
    },
    {
      path: '/',
      component: MainLayout,
      children: [
        {
          path: '',
          name: 'dashboard',
          component: DashboardView,
        },
        // Tờ khai
        {
          path: 'invoice/declaration/create',
          name: 'invoice-declaration-create',
          meta: { noPadding: true },
          component: InvoiceDeclaration,
        },
        {
          path: 'invoice/declaration/:id',
          name: 'invoice-declaration-edit',
          meta: { noPadding: true },
          component: InvoiceDeclaration,
        },
        {
          path: 'invoice/declaration/view/:id',
          name: 'invoice-declaration-view',
          meta: { noPadding: true },
          component: InvoiceDeclaration,
        },
        {
          path: 'invoice/declaration/list',
          name: 'invoice-declaration-list',
          component: DeclarationList,
        },
        // Mẫu hoá đơn
        {
          path: 'invoice/template/list',
          name: 'invoice-template-list',
          component: TemplateList,
        },

        // Hoá dơn
        {
          path: 'invoice/creation',
          name: 'invoice-creation',
          meta: { noPadding: true },
          component: InvoiceCreation,
        },
        {
          path: 'invoice/creation/:id',
          name: 'invoice-creation-edit',
          meta: { noPadding: true },
          component: InvoiceCreation,
        },
        {
          path: 'invoice/list',
          name: 'invoice-list',
          component: ShiftView,
        },
      ],
    },

    {
      path: '/invoice/template/setup',
      name: 'invoice-template-setup',
      component: TemplateSetup,
    },
    {
      path: '/invoice/template/editor',
      name: 'invoice-template-editor',
      component: TemplateEditor,
    },
    // Template edit route
    {
      path: '/invoice/template/:id',
      name: 'invoice-template-edit',
      component: TemplateEditor,
    },
    // Admin routes - separate route with MainLayout
    {
      path: '/admin',
      component: MainLayout,
      children: [
        {
          path: 'companies',
          name: 'admin-companies',
          component: CompanyList,
        },
        {
          path: 'users',
          name: 'admin-users',
          component: UserList,
        },
        {
          path: 'assign-user',
          name: 'admin-assign-user',
          component: AssignUserToCompany,
        },
      ],
    },

    {
      path: '/login',
      name: 'login',
      component: LoginView,
    },
    // 404 - Not Found (catch-all)
    {
      path: '/:pathMatch(.*)*',
      name: 'not-found',
      component: NotFound,
    },
  ],
})

export default router
