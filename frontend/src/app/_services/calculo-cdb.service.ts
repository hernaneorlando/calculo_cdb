import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CalculoCdb } from '../_models/calculo-cdb.model';
import { Observable } from 'rxjs';
import { CalculoCdbDto } from '../_models/calculo-cdb-dto.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CalculoCdbService {

  constructor(private http: HttpClient) { }

  calculaCdb(calculoCdb: CalculoCdb): Observable<CalculoCdbDto> {
    return this.http.post<CalculoCdbDto>(`${environment.apiUrl}/cdb`, calculoCdb);
  }
}
