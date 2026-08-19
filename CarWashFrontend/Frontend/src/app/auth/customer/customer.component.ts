import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-customer',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './customer.component.html',
  styleUrl: './customer.component.css'
})
export class CustomerComponent implements OnInit {
  customerName = 'Customer';
  customerFullName = '';
  customerEmail = '';
  customerPhone = '';
  section: string = 'dashboard';
  hasCompletedOrders: boolean = false;

  // Cars management
  cars: { id: number; make: string; model: string; year: number }[] = [];
  newCar = { make: '', model: '', year: new Date().getFullYear() };

  // Packages, Promo Codes, Add Ons
  packages: any[] = [];
  promoCodes: any[] = [];
  addOns: any[] = [];

  // Orders
  orders: any[] = [];

  // Payment receipts
  paymentReceipts: any[] = [];

  // Total amount for payment section (all orders)
  totalAmount: number = 0;
  // Payment section properties
  paymentMethod: string = '';
  upiId: string = '';
  cardNumber: string = '';
  expiry: string = '';
  cvv: string = '';
  paymentSuccess: boolean = false;
  paymentError: string = '';
  paymentLoading: boolean = false;

  // Edit car state
  editingCarIndex: number | null = null;
  editingCar = { id: 0, make: '', model: '', year: new Date().getFullYear() };

  private apiUrl = 'http://localhost:5172/api/Car';
  private packageUrl = 'http://localhost:5172/api/ServicePackage';
  private promoCodeUrl = 'http://localhost:5172/api/PromoCode';
  private addOnUrl = 'http://localhost:5172/api/AddOn';
  private orderUrl = 'http://localhost:5172/api/Order';
  private ratingUrl = 'http://localhost:5172/api/Rating';
  private paymentReceiptUrl = 'http://localhost:5172/api/PaymentReceipt';

  constructor(private router: Router, private http: HttpClient) {}

  ngOnInit(): void {
    this.fetchCars();
    this.fetchPackages();
    this.fetchPromoCodes();
    this.fetchAddOns();
    this.fetchOrders();
    this.fetchPaymentReceipts();
    const token = localStorage.getItem('token');
    if (token) {
      const payload = JSON.parse(atob(token.split('.')[1]));
      this.customerName = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
      this.customerFullName = payload['fullName'] || this.customerName;
      this.customerEmail = payload['email'] || '';
      this.customerPhone = payload['phone'] || '';
    }
  }

  setSection(section: string) {
    this.section = section;
    if (section === 'bookings' || section === 'payment' || section === 'rate') {
      this.fetchOrders();
      this.fetchPaymentReceipts();
    }
  }

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }

  get upcomingWashesCount(): number {
    return Array.isArray(this.orders)
      ? this.orders.filter(order => order.status === 'PENDING').length
      : 0;
  }

  get completedWashesCount(): number {
    return Array.isArray(this.orders)
      ? this.orders.filter(order => order.status === 'COMPLETED').length
      : 0;
  }

  get cancelledWashesCount(): number {
    return Array.isArray(this.orders)
      ? this.orders.filter(order => order.status === 'DECLINED').length
      : 0;
  }

  // Getter for template to check if there are no completed orders
  get hasNoCompletedOrders(): boolean {
    return Array.isArray(this.orders)
      ? this.orders.filter(order => order.status === 'COMPLETED').length === 0
      : true;
  }

  // Only unpaid completed orders (no payment receipt exists)
  get unpaidCompletedOrders() {
    return Array.isArray(this.orders)
      ? this.orders.filter(order =>
          order.status === 'COMPLETED' &&
          !this.paymentReceipts.some(receipt => receipt.orderId === order.id)
        )
      : [];
  }

  // Total amount for unpaid completed orders
  get totalCompletedAmount(): number {
    return this.unpaidCompletedOrders.reduce((sum, order) => sum + Number(order.totalAmount || 0), 0);
  }

  // Fetch all cars from the backend
  fetchCars() {
    const headers = this.getAuthHeaders();
    this.http.get<{ id: number; make: string; model: string; year: number }[]>(this.apiUrl, { headers }).subscribe({
      next: (data) => {
        this.cars = data;
      },
      error: (err) => {
        console.error('Error fetching cars:', err);
      }
    });
  }

  // Fetch all packages
  fetchPackages() {
    const headers = this.getAuthHeaders();
    this.http.get<any[]>(this.packageUrl, { headers }).subscribe({
      next: (data) => {
        this.packages = data;
      },
      error: (err) => {
        console.error('Error fetching packages:', err);
      }
    });
  }

  // Fetch all promo codes
  fetchPromoCodes() {
    const headers = this.getAuthHeaders();
    this.http.get<any[]>(this.promoCodeUrl, { headers }).subscribe({
      next: (data) => {
        this.promoCodes = data;
      },
      error: (err) => {
        console.error('Error fetching promo codes:', err);
      }
    });
  }

  // Fetch all add ons
  fetchAddOns() {
    const headers = this.getAuthHeaders();
    this.http.get<any[]>(this.addOnUrl, { headers }).subscribe({
      next: (data) => {
        this.addOns = data;
      },
      error: (err) => {
        console.error('Error fetching add ons:', err);
      }
    });
  }

  // Fetch all orders and their ratings
  fetchOrders() {
    const headers = this.getAuthHeaders();
    this.http.get<any[]>(this.orderUrl, { headers }).subscribe({
      next: (data) => {
        this.orders = data;
        this.calculateTotalAmount();
        this.hasCompletedOrders = Array.isArray(this.orders)
          ? this.orders.some(order => order.status === 'COMPLETED')
          : false;
        this.fetchRatingsForOrders();
      },
      error: (err) => {
        console.error('Error fetching orders:', err);
        this.totalAmount = 0;
        this.hasCompletedOrders = false;
      }
    });
  }

  // Fetch all payment receipts
  fetchPaymentReceipts() {
    const headers = this.getAuthHeaders();
    this.http.get<any[]>(this.paymentReceiptUrl, { headers }).subscribe({
      next: (data) => {
        this.paymentReceipts = data;
      },
      error: (err) => {
        console.error('Error fetching payment receipts:', err);
        this.paymentReceipts = [];
      }
    });
  }

  // Submit or update a rating for an order
  submitRating(order: any) {
    order.ratingLoading = true;
    order.ratingMessage = '';
    // You may need to set userId and washerId appropriately
    const ratingDto = {
      orderId: order.id,
      userId: order.userId || '',      // Set this as needed
      washerId: order.washerId || '',  // Set this as needed
      ratingValue: order.ratingValue
    };
    const headers = this.getAuthHeaders();
    if (order.ratingId) {
      // Update
      this.http.put(`${this.ratingUrl}/${order.ratingId}`, ratingDto, { headers }).subscribe({
        next: () => {
          order.ratingMessage = 'Rating updated!';
          order.ratingLoading = false;
        },
        error: () => {
          order.ratingMessage = 'Failed to update rating.';
          order.ratingLoading = false;
        }
      });
    } else {
      // Create
      this.http.post(this.ratingUrl, ratingDto, { headers }).subscribe({
        next: (res: any) => {
          order.ratingMessage = 'Rating submitted!';
          order.ratingId = res.id;
          order.ratingLoading = false;
        },
        error: () => {
          order.ratingMessage = 'Failed to submit rating.';
          order.ratingLoading = false;
        }
      });
    }
  }

  // Fetch all ratings and map to orders
  fetchRatingsForOrders() {
    const headers = this.getAuthHeaders();
    this.http.get<any[]>(this.ratingUrl, { headers }).subscribe({
      next: (ratings) => {
        this.orders.forEach(order => {
          const rating = ratings.find(r => r.orderId === order.id);
          if (rating) {
            order.ratingId = rating.id;
            order.ratingValue = rating.ratingValue; // Use ratingValue from API
          } else {
            order.ratingId = null;
            order.ratingValue = null;
          }
          order.ratingMessage = '';
          order.ratingLoading = false;
        });
      },
      error: (err) => {
        console.error('Error fetching ratings:', err);
      }
    });
  }

  // Delete a rating for an order
  deleteRating(order: any) {
    if (!order.ratingId) return;
    order.ratingLoading = true;
    const headers = this.getAuthHeaders();
    this.http.delete(`${this.ratingUrl}/${order.ratingId}`, { headers }).subscribe({
      next: () => {
        order.ratingMessage = 'Rating deleted!';
        order.ratingId = null;
        order.ratingValue = null;
        order.ratingLoading = false;
      },
      error: () => {
        order.ratingMessage = 'Failed to delete rating.';
        order.ratingLoading = false;
      }
    });
  }

  // Calculate total amount for all orders
  calculateTotalAmount() {
    this.totalAmount = Array.isArray(this.orders)
      ? this.orders.reduce((sum, order) => sum + Number(order.totalAmount || 0), 0)
      : 0;
  }

  // Add a new car
  addCar() {
    if (this.newCar.make && this.newCar.model && this.newCar.year) {
      const headers = this.getAuthHeaders();
      this.http.post(this.apiUrl, this.newCar, { headers }).subscribe({
        next: (car: any) => {
          this.cars.push(car);
          this.newCar = { make: '', model: '', year: new Date().getFullYear() };
        },
        error: (err) => {
          alert('Failed to add car.');
          console.error('Error adding car:', err);
        }
      });
    }
  }

  // Edit a car
  editCar(index: number) {
    this.editingCarIndex = index;
    this.editingCar = { ...this.cars[index] };
  }

  // Update a car
  updateCar() {
    if (this.editingCarIndex !== null) {
      const headers = this.getAuthHeaders();
      this.http.put(`${this.apiUrl}/${this.editingCar.id}`, this.editingCar, { headers }).subscribe({
        next: () => {
          this.cars[this.editingCarIndex!] = { ...this.editingCar };
          this.cancelEdit();
        },
        error: (err) => {
          console.error('Error updating car:', err);
        }
      });
    }
  }

  // Cancel editing
  cancelEdit() {
    this.editingCarIndex = null;
    this.editingCar = { id: 0, make: '', model: '', year: new Date().getFullYear() };
  }

  // Delete a car
  deleteCar(index: number) {
    const carId = this.cars[index].id;
    const headers = this.getAuthHeaders();
    this.http.delete(`${this.apiUrl}/${carId}`, { headers }).subscribe({
      next: () => {
        this.cars.splice(index, 1);
      },
      error: (err) => {
        console.error('Error deleting car:', err);
      }
    });
  }

  // Helper method to get authorization headers
  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }

  showBookWash = false;
  washBooking = {
    carId: null,
    packageId: null,
    promoCodeId: null,
    scheduledDate: new Date().toISOString().substring(0, 10),
    location: '',
    addOnIds: [] as number[]
  };

  bookNewWash() {
    this.showBookWash = true;
    this.washBooking = {
      carId: null,
      packageId: null,
      promoCodeId: null,
      scheduledDate: new Date().toISOString().substring(0, 10),
      location: '',
      addOnIds: []
    };
  }

  submitWashBooking() {
    if (
      Array.isArray(this.washBooking.addOnIds) &&
      (this.washBooking.addOnIds.length === 0 ||
        (this.washBooking.addOnIds.length === 1 && !this.washBooking.addOnIds[0]))
    ) {
      this.washBooking.addOnIds = [];
    }

    const headers = this.getAuthHeaders();
    this.http.post(this.orderUrl, this.washBooking, { headers }).subscribe({
      next: (res) => {
        alert('Order placed successfully!');
        this.showBookWash = false;
        if (this.section === 'bookings' || this.section === 'payment' || this.section === 'rate') {
          this.fetchOrders();
        }
      },
      error: (err) => {
        alert('Failed to place order.');
        console.error('Order error:', err);
      }
    });
  }

  // Payment logic
  makePayment() {
    this.paymentSuccess = false;
    this.paymentError = '';
    this.paymentLoading = true;

    // Always refresh payment receipts before making payment
    this.fetchPaymentReceipts();
    setTimeout(() => {
      const unpaidOrders = this.unpaidCompletedOrders;
      if (unpaidOrders.length === 0) {
        this.paymentError = 'No unpaid completed orders to pay for.';
        this.paymentLoading = false;
        return;
      }

      // Validate payment method
      if (
        (this.paymentMethod === 'upi' && !this.upiId) ||
        (this.paymentMethod === 'card' && (!this.cardNumber || !this.expiry || !this.cvv))
      ) {
        this.paymentError = 'Payment failed. Please check your details and try again.';
        this.paymentLoading = false;
        return;
      }

      const headers = this.getAuthHeaders();
      let paidCount = 0;
      let failed = false;

      unpaidOrders.forEach(order => {
        // Double-check before payment: skip if receipt exists
        if (this.paymentReceipts.some(receipt => receipt.orderId === order.id)) {
          return;
        }
        const paymentDto = {
          orderId: order.id,
          amountPaid: order.totalAmount,
          paymentStatus: 'PAID'
        };
        this.http.post(this.paymentReceiptUrl, paymentDto, { headers }).subscribe({
          next: () => {
            paidCount++;
            if (paidCount === unpaidOrders.length && !failed) {
              this.paymentSuccess = true;
              this.paymentError = '';
              this.paymentLoading = false;
              // Clear bookings section and set totalAmount to 0 after payment
              this.orders = [];
              this.totalAmount = 0;
              this.fetchPaymentReceipts();
            }
          },
          error: () => {
            failed = true;
            this.paymentSuccess = false;
            this.paymentError = 'Payment failed for one or more orders.';
            this.paymentLoading = false;
          }
        });
      });
    }, 500); // Give fetchPaymentReceipts a moment to update
  }
}