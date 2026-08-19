import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-washer',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './washer.component.html',
  styleUrls: ['./washer.component.css']
})
export class WasherComponent implements OnInit {
  section: string = 'washRequests';
  washerName: string = 'Washer';
  washRequests: any[] = [];
  orders: any[] = [];
  AspNetusers: any[] = [];

  constructor(private router: Router, private http: HttpClient) {
    const token = localStorage.getItem('token');
    if (token) {
      const payload = JSON.parse(atob(token.split('.')[1]));
      this.washerName = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
    }
  }

  ngOnInit(): void {
    this.fetchWasherRequests();
  }

  setSection(section: string) {
    this.section = section;
    if (section === 'washRequests') {
      this.fetchWasherRequests();
    }
  }

  fetchWasherRequests() {
    const headers = this.getAuthHeaders();
    this.http.get<any[]>('http://localhost:5172/api/WashRequest/washer', { headers }).subscribe({
      next: (data) => {
        this.washRequests = data;
        this.fetchOrdersAndMerge();
        console.log('Washer requests:', this.washRequests);
      },
      error: (err) => {
        console.error('Error fetching washer requests:', err);
      }
    });
  }

  fetchOrdersAndMerge() {
  const headers = this.getAuthHeaders();
  this.http.get<any[]>('http://localhost:5172/api/WashRequest/washer', { headers }).subscribe({
      next: (data) => {
      this.washRequests = data;
      this.washRequests = this.washRequests.map(req => {
        // Find the order for this wash request's userId and carId
        const userOrder = this.AspNetusers.find(o => o.userId === req.userId);
        const carOrder = this.orders.find(o => o.carId === req.carId);

        const customerName = userOrder?.customer?.fullName || userOrder?.customer?.name || '';
        const location = userOrder?.location || '';
        const carName = carOrder?.car ? (carOrder.car.make + ' ' + carOrder.car.model) : '';

        return {
          ...req,
          customerName,
          car: carName,
          date: req.date, // or req.scheduledDate if present
          location,
        };
      });
    },
    error: (err) => {
      console.error('Error fetching orders:', err);
    }
  });
}

  updateStatus(request: any) {
  const headers = this.getAuthHeaders().set('Content-Type', 'application/json');
  this.http.put(
    `http://localhost:5172/api/WashRequest/status/${request.orderId}`,
    JSON.stringify(request.status), // send as raw string
    { headers }
  ).subscribe({
    next: () => {
      this.fetchWasherRequests();
    },
    error: (err) => {
      console.error('Error updating status:', err);
    }
  });
}

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }
}