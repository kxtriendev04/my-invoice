<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" encoding="UTF-8" indent="yes"/>

	<xsl:template match="/">
		<html>
			<head>
				<meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/>
				<style>
					@page { size: A4; margin: 0; }
					body {
					font-family: "Times New Roman", Times, serif;
					font-size: 13px;
					margin: 8mm;
					color: #000;
					}

					.invoice-container {
					width: 190mm;
					min-height: 275mm;
					margin: 0 auto;
					background: #fff;
					padding: 3mm;
					border: 18px solid transparent;
					border-image-source: url('https://www.meinvoice.vn/tin-tuc/wp-content/uploads/2019/10/mau-hoa-don-dien-tu-misa-meinvoice.png');
					border-image-slice: 30;
					border-image-repeat: round;
					position: relative;
					box-shadow: 0 0 10px rgba(0,0,0,0.1);
					}

					.inner-content {
					border: 1px solid #0078d4;
					padding: 8mm;
					min-height: 255mm;
					position: relative;
					}

					table { width: 100%; border-collapse: collapse; margin-bottom: 8px; }
					.text-center { text-align: center; }
					.text-right { text-align: right; }
					.bold { font-weight: bold; }
					.uppercase { text-transform: uppercase; }

					/* Header Style */
					.company-name { font-size: 16px; color: #000; font-weight: bold; line-height: 1.4; }
					.invoice-title { font-size: 24px; color: #000; font-weight: bold; margin: 10px 0 5px 0; }

					/* Table Style */
					.items-table th {
					border: 1px solid #000;
					padding: 8px 5px;
					background: #fff;
					font-weight: bold;
					}
					.items-table td { border: 1px solid #000; padding: 6px 5px; vertical-align: middle; }

					.signature-box {
					border: 1px solid #28a745;
					color: #d32f2f;
					padding: 8px;
					width: 178px;
					text-align: left;
					font-size: 11px;
					background: #f9fff9;
					margin-top: 8px;
					display: inline-block;
					line-height: 1.4;
					}

					.footer-info {
					position: absolute;
					bottom: 5mm;
					left: 0;
					right: 0;
					text-align: center;
					font-size: 12px;
					line-height: 1.5;
					}

					.qr-img { width: 85px; height: 85px; border: 1px solid #eee; }
					hr { border: 0; border-top: 1px solid #000; margin: 12px 0; }
				</style>
			</head>
			<body>
				<div class="invoice-container">
					<div class="inner-content">
						<table>
							<tr>
								<td width="75%" style="vertical-align: top;">
									<div class="company-name uppercase company-name-content">
										<xsl:value-of select="/HDon/DLHDon/TTChung/NBan/Ten"/>
									</div>
									<div style="margin-top: 4px;">
										Mã số thuế: <xsl:value-of select="/HDon/DLHDon/TTChung/NBan/MST"/> <span class="tax-code-content"></span>
									</div>
									<div style="margin-top: 2px;">
										Địa chỉ: <xsl:value-of select="/HDon/DLHDon/TTChung/NBan/DChi"/> <span class="seller-address-content"></span>
									</div>
									<div style="margin-top: 2px;">
										Điện thoại: <xsl:value-of select="/HDon/DLHDon/TTChung/NBan/SDThoai"/> <span class="seller-phone-content"></span>
									</div>
								</td>
								<td width="25%" class="text-right">
									<img class="qr-img" src="https://api.qrserver.com/v1/create-qr-code/?size=85x85&amp;data={/HDon/DLHDon/TTChung/SHDon}" />
								</td>
							</tr>
						</table>

						<div class="text-center">
							<div class="invoice-title uppercase">Hóa đơn giá trị gia tăng</div>
							<div style="margin-bottom: 5px; font-style: italic;" class="date-content">
								Ngày <xsl:value-of select="substring(/HDon/DLHDon/TTChung/NLap, 9, 2)"/>
								tháng <xsl:value-of select="substring(/HDon/DLHDon/TTChung/NLap, 6, 2)"/>
								năm <xsl:value-of select="substring(/HDon/DLHDon/TTChung/NLap, 1, 4)"/>
							</div>
							<div class="text-right" style="margin-top: -35px;">
								<div>
									Ký hiệu: <span class="bold inv-series-content">
										<xsl:value-of select="/HDon/DLHDon/TTChung/KHHDon"/>
									</span>
								</div>
								<div>
									Số: <span class="bold inv-no-content" style="font-size: 16px;">
										<xsl:value-of select="/HDon/DLHDon/TTChung/SHDon"/>
									</span>
								</div>
							</div>
						</div>

						<hr/>

						<table style="line-height: 1.7;">
							<tr>
								<td width="20%">Họ tên người mua hàng:</td>
								<td class="bold">
									<xsl:value-of select="/HDon/DLHDon/TTChung/NMua/HVTNMHang"/>
								</td>
							</tr>
							<tr>
								<td>Tên đơn vị:</td>
								<td class="uppercase bold">
									<xsl:value-of select="/HDon/DLHDon/TTChung/NMua/Ten"/>
								</td>
							</tr>
							<tr>
								<td>Mã số thuế:</td>
								<td class="bold">
									<xsl:value-of select="/HDon/DLHDon/TTChung/NMua/MST"/>
								</td>
							</tr>
							<tr>
								<td>Địa chỉ:</td>
								<td>
									<xsl:value-of select="/HDon/DLHDon/TTChung/NMua/DChi"/>
								</td>
							</tr>
							<tr>
								<td>Hình thức thanh toán:</td>
								<td>
									<xsl:value-of select="/HDon/DLHDon/TTChung/HTTToan"/>
									<span style="margin-left: 60px;">
										Số tài khoản: <xsl:value-of select="/HDon/DLHDon/TTChung/NMua/STKhoan"/>
									</span>
								</td>
							</tr>
						</table>

						<table class="items-table" style="margin-top: 15px;">
							<thead>
								<tr>
									<th width="5%">STT</th>
									<th width="40%">Tên hàng hóa, dịch vụ</th>
									<th width="10%">ĐVT</th>
									<th width="10%">Số lượng</th>
									<th width="15%">Đơn giá</th>
									<th width="20%">Thành tiền</th>
								</tr>
							</thead>
							<tbody>
								<xsl:for-each select="/HDon/DLHDon/DSHHDVu/HHDVu">
									<tr>
										<td class="text-center">
											<xsl:value-of select="STT"/>
										</td>
										<td>
											<xsl:value-of select="THHDVu"/>
										</td>
										<td class="text-center">
											<xsl:value-of select="DVTinh"/>
										</td>
										<td class="text-right">
											<xsl:value-of select="SLuong"/>
										</td>
										<td class="text-right">
											<xsl:value-of select="DGia"/>
										</td>
										<td class="text-right bold">
											<xsl:value-of select="TTien"/>
										</td>
									</tr>
								</xsl:for-each>
							</tbody>
						</table>

						<table style="margin-top: -9px;">
							<tr>
								<td width="80%" class="text-right" style="padding: 7px 5px; border: 1px solid #000;">Cộng tiền hàng:</td>
								<td class="text-right bold" style="padding: 7px 5px; border: 1px solid #000;">
									<xsl:value-of select="/HDon/DLHDon/TToan/THTToan"/>
								</td>
							</tr>
							<tr>
								<td class="text-right" style="padding: 7px 5px; border: 1px solid #000;">
									Thuế suất GTGT: <xsl:value-of select="/HDon/DLHDon/DSHHDVu/HHDVu[1]/TSuat"/> - Tiền thuế GTGT:
								</td>
								<td class="text-right bold" style="padding: 7px 5px; border: 1px solid #000;">
									<xsl:value-of select="/HDon/DLHDon/TToan/TGTGT"/>
								</td>
							</tr>
							<tr>
								<td class="text-right bold" style="padding: 7px 5px; border: 1px solid #000;">Tổng cộng tiền thanh toán:</td>
								<td class="text-right bold" style="padding: 7px 5px; border: 1px solid #000; font-size: 15px;">
									<xsl:value-of select="/HDon/DLHDon/TToan/TgTCien"/>
								</td>
							</tr>
						</table>

						<div style="margin-top: 12px;">
							Số tiền viết bằng chữ: <span class="bold" style="font-style: italic;">
								<xsl:value-of select="/HDon/DLHDon/TToan/TgTienBangChu"/>
							</span>
						</div>

						<table style="margin-top: 30px;">
							<tr class="text-center">
								<td width="50%" style="vertical-align: top;">
									<div class="bold uppercase">Người mua hàng</div>
									<div style="font-style: italic; font-size: 11px;">(Chữ ký số (nếu có))</div>
								</td>
								<td width="50%" style="vertical-align: top;">
									<div class="bold uppercase">Người bán hàng</div>
									<div style="font-style: italic; font-size: 11px;">(Chữ ký điện tử, Chữ ký số)</div>
									<div class="signature-box">
										<div style="color: #28a745; font-weight: bold; border-bottom: 1px solid #28a745; margin-bottom: 4px; text-align: center;">Signature Valid</div>
										<div>
											Ký bởi: <xsl:value-of select="/HDon/DLHDon/TTChung/NBan/Ten"/>
										</div>
										<div>
											Ký ngày: <xsl:value-of select="/HDon/DLHDon/TTChung/NLap"/>
										</div>
									</div>
								</td>
							</tr>
						</table>

						<div class="footer-info">
							<div>
								Tra cứu tại Website: <span style="color: blue; text-decoration: underline;">https://www.myinvoice.vn/tra-cuu</span> - Mã tra cứu: .....................
							</div>
							<div style="font-style: italic; font-size: 11px;">(Cần kiểm tra, đối chiếu khi lập, giao, nhận hóa đơn)</div>
							<div class="bold" style="margin-top: 6px;">
								Phát hành bởi phần mềm MyInvoice - Công ty Chuyển đổi số Net Software
							</div>
						</div>
					</div>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>