import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class DataService {
    public _pageSize: number = 10;
    public _baseUri: string = 'https://api.example.com';

    constructor(public http: HttpClient) {

    }
    set(baseUri: string, pageSize: number): void {
        this._baseUri = baseUri;
        this._pageSize = pageSize;
    }
    get(page: number) {
        var uri = this._baseUri + page.toString() + '/' + this._pageSize.toString();
        return this.http.get(uri, { observe: 'response' as 'response' }) as any as Observable<HttpResponse<any>>;
    }
    post(data?: any, mapJson: boolean = true) {
        if (mapJson)
            return this.http.post<any>(this._baseUri, data);
        else
            return this.http.post(this._baseUri, data, { observe: 'response' as 'response' });
    }
    delete(id: number) {
        return this.http.delete<any>(this._baseUri + '/' + id.toString());
    }

    deleteResource(resource: string) {
        return this.http.delete<any>(resource);
    }
    getResponse(url: string) {
        return this.http.get(url, { observe: 'response' as 'response' }) as any as Observable<HttpResponse<any>>;
    }
}