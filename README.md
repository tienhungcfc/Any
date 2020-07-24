# Any
> Dữ liệu động, theo cây
###### Sử dụng cơ bản
```javascript
dynamic a = new Any(); /// {}
a.a = 0; /// {"a": 0}
a.a = "title"; /// {"a": "title"}
a.a = true; /// {"a": true}
a.a = new object[] {0,1}; /// {"a": [0,1]}
a.a[0].title = "tille a"; /// {"a": [{"title": "title a"}, 1]}
```
