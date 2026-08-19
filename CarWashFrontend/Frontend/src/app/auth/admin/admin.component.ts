import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  section: string = 'assignWasher';
  adminName: string = 'Admin';

  // Assign Washer
  assignOrderId: number | null = null;
  assignWasherId: string | number | null = null;
  assignWasherMessage: string = '';

  // Service Packages
  servicePackages: any[] = [];
  servicePackageFormModel: any = { id: null, name: '', description: '', price: null };
  servicePackageMessage: string = '';

  // Promo Codes
  promoCodes: any[] = [];
  promoCodeFormModel: any = { id: null, code: '', discountPercent: null, validTill: '' };
  promoCodeMessage: string = '';

  // Add Ons
  addOns: any[] = [];
  addOnFormModel: any = { id: null, name: '', price: null, isActive: true };
  addOnMessage: string = '';

  constructor(private router: Router, private http: HttpClient) {
    const token = localStorage.getItem('token');
    if (token) {
      const payload = JSON.parse(atob(token.split('.')[1]));
      this.adminName = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
    }
  }

  ngOnInit(): void {
    this.getServicePackages();
    this.getPromoCodes();
    this.getAddOns();
  }

  setSection(section: string) {
    this.section = section;
    if (section === 'servicePackages') {
      this.getServicePackages();
    }
    if (section === 'promoCodes') {
      this.getPromoCodes();
    }
    if (section === 'addOns') {
      this.getAddOns();
    }
  }

  // Assign Washer
  assignWasher() {
    const headers = this.getAuthHeaders();
    this.assignWasherMessage = '';
    this.http.post(
      'http://localhost:5172/api/Washer/assign',
      {
        OrderId: this.assignOrderId,
        WasherId: this.assignWasherId
      },
      { headers, responseType: 'text' }
    ).subscribe({
      next: (res) => {
        this.assignWasherMessage = res || 'Washer assigned successfully!';
        setTimeout(() => {
          this.assignOrderId = null;
          this.assignWasherId = null;
          this.assignWasherMessage = '';
        }, 2000);
      },
      error: (err) => {
        this.assignWasherMessage = 'Failed to assign washer.';
        console.error('Error assigning washer:', err);
        alert(JSON.stringify(err.error));
      }
    });
  }

  // Service Package CRUD
  getServicePackages() {
    const headers = this.getAuthHeaders();
    this.http.get<any[]>('http://localhost:5172/api/ServicePackage', { headers }).subscribe({
      next: (data) => this.servicePackages = data,
      error: (err) => console.error('Error fetching service packages:', err)
    });
  }

  saveServicePackage() {
    const headers = this.getAuthHeaders();
    if (this.servicePackageFormModel.id) {
      // Update
      this.http.put(
        `http://localhost:5172/api/ServicePackage/${this.servicePackageFormModel.id}`,
        this.servicePackageFormModel,
        { headers }
      ).subscribe({
        next: () => {
          this.servicePackageMessage = 'Service package updated!';
          this.getServicePackages();
          this.resetServicePackageForm();
          setTimeout(() => this.servicePackageMessage = '', 2000);
        },
        error: (err) => {
          this.servicePackageMessage = 'Failed to update package.';
          console.error(err);
        }
      });
    } else {
      // Create
      this.http.post(
        'http://localhost:5172/api/ServicePackage',
        this.servicePackageFormModel,
        { headers }
      ).subscribe({
        next: () => {
          this.servicePackageMessage = 'Service package created!';
          this.getServicePackages();
          this.resetServicePackageForm();
          setTimeout(() => this.servicePackageMessage = '', 2000);
        },
        error: (err) => {
          this.servicePackageMessage = 'Failed to create package.';
          console.error(err);
        }
      });
    }
  }

  editServicePackage(pkg: any) {
    this.servicePackageFormModel = { ...pkg };
  }

  deleteServicePackage(id: any) {
    const headers = this.getAuthHeaders();
    if (confirm('Are you sure you want to delete this service package?')) {
      this.http.delete(`http://localhost:5172/api/ServicePackage/${id}`, { headers }).subscribe({
        next: () => {
          this.servicePackageMessage = 'Service package deleted!';
          this.getServicePackages();
          setTimeout(() => this.servicePackageMessage = '', 2000);
        },
        error: (err) => {
          this.servicePackageMessage = 'Failed to delete package.';
          console.error(err);
        }
      });
    }
  }

  resetServicePackageForm() {
    this.servicePackageFormModel = { id: null, name: '', description: '', price: null };
  }

  // Promo Code CRUD
  getPromoCodes() {
    const headers = this.getAuthHeaders();
    this.http.get<any[]>('http://localhost:5172/api/PromoCode', { headers }).subscribe({
      next: (data) => this.promoCodes = data,
      error: (err) => console.error('Error fetching promo codes:', err)
    });
  }

  savePromoCode() {
    const headers = this.getAuthHeaders();
    // Ensure validTill is in ISO format
    if (this.promoCodeFormModel.validTill) {
      this.promoCodeFormModel.validTill = new Date(this.promoCodeFormModel.validTill).toISOString();
    }
    if (this.promoCodeFormModel.id) {
      // Update
      this.http.put(
        `http://localhost:5172/api/PromoCode/${this.promoCodeFormModel.id}`,
        this.promoCodeFormModel,
        { headers }
      ).subscribe({
        next: () => {
          this.promoCodeMessage = 'Promo code updated!';
          this.getPromoCodes();
          this.resetPromoCodeForm();
          setTimeout(() => this.promoCodeMessage = '', 2000);
        },
        error: (err) => {
          this.promoCodeMessage = 'Failed to update promo code.';
          console.error(err);
        }
      });
    } else {
      // Create
      this.http.post(
        'http://localhost:5172/api/PromoCode',
        this.promoCodeFormModel,
        { headers }
      ).subscribe({
        next: () => {
          this.promoCodeMessage = 'Promo code created!';
          this.getPromoCodes();
          this.resetPromoCodeForm();
          setTimeout(() => this.promoCodeMessage = '', 2000);
        },
        error: (err) => {
          this.promoCodeMessage = 'Failed to create promo code.';
          console.error(err);
        }
      });
    }
  }

  editPromoCode(promo: any) {
    // Convert validTill to yyyy-MM-dd for input[type=date]
    const validTill = promo.validTill ? promo.validTill.substring(0, 10) : '';
    this.promoCodeFormModel = { ...promo, validTill };
  }

  deletePromoCode(id: any) {
    const headers = this.getAuthHeaders();
    if (confirm('Are you sure you want to delete this promo code?')) {
      this.http.delete(`http://localhost:5172/api/PromoCode/${id}`, { headers }).subscribe({
        next: () => {
          this.promoCodeMessage = 'Promo code deleted!';
          this.getPromoCodes();
          setTimeout(() => this.promoCodeMessage = '', 2000);
        },
        error: (err) => {
          this.promoCodeMessage = 'Failed to delete promo code.';
          console.error(err);
        }
      });
    }
  }

  resetPromoCodeForm() {
    this.promoCodeFormModel = { id: null, code: '', discountPercent: null, validTill: '' };
  }

  // Add On CRUD
  getAddOns() {
    const headers = this.getAuthHeaders();
    this.http.get<any[]>('http://localhost:5172/api/AddOn', { headers }).subscribe({
      next: (data) => this.addOns = data,
      error: (err) => console.error('Error fetching add ons:', err)
    });
  }

  saveAddOn() {
    const headers = this.getAuthHeaders();
    if (this.addOnFormModel.id) {
      // Update
      this.http.put(
        `http://localhost:5172/api/AddOn/${this.addOnFormModel.id}`,
        this.addOnFormModel,
        { headers }
      ).subscribe({
        next: () => {
          this.addOnMessage = 'Add On updated!';
          this.getAddOns();
          this.resetAddOnForm();
          setTimeout(() => this.addOnMessage = '', 2000);
        },
        error: (err) => {
          this.addOnMessage = 'Failed to update add on.';
          console.error(err);
        }
      });
    } else {
      // Create
      this.http.post(
        'http://localhost:5172/api/AddOn',
        this.addOnFormModel,
        { headers }
      ).subscribe({
        next: () => {
          this.addOnMessage = 'Add On created!';
          this.getAddOns();
          this.resetAddOnForm();
          setTimeout(() => this.addOnMessage = '', 2000);
        },
        error: (err) => {
          this.addOnMessage = 'Failed to create add on.';
          console.error(err);
        }
      });
    }
  }

  editAddOn(addOn: any) {
    this.addOnFormModel = { ...addOn };
  }

  deleteAddOn(id: any) {
    const headers = this.getAuthHeaders();
    if (confirm('Are you sure you want to delete this add on?')) {
      this.http.delete(`http://localhost:5172/api/AddOn/${id}`, { headers }).subscribe({
        next: () => {
          this.addOnMessage = 'Add On deleted!';
          this.getAddOns();
          setTimeout(() => this.addOnMessage = '', 2000);
        },
        error: (err) => {
          this.addOnMessage = 'Failed to delete add on.';
          console.error(err);
        }
      });
    }
  }

  resetAddOnForm() {
    this.addOnFormModel = { id: null, name: '', price: null, isActive: true };
  }

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }
}