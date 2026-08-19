import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Car } from '../models/car.model';

@Injectable({
  providedIn: 'root'
})
export class CarService {
  private apiUrl = 'http://localhost:5172/api/Car'; // Replace with actual backend URL

  constructor(private http: HttpClient) {}

  getAllCars(): Observable<Car[]> {
    return this.http.get<Car[]>(this.apiUrl);
  }

  getCarById(id: number): Observable<Car> {
    return this.http.get<Car>(`${this.apiUrl}/${id}`);
  }

  addCar(car: Car, headers: any) {
    return this.http.post<Car>(this.apiUrl, car, { headers });
}

  updateCar(id: number, car: Car): Observable<Car> {
    return this.http.put<Car>(`${this.apiUrl}/${id}`, car);
  }

  deleteCar(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}