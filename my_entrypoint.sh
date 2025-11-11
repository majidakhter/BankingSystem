#!/bin/sh
# Add your custom logic here (e.g., waiting for database)
echo "Running custom entrypoint logic..."
# Then, execute kc.sh to start Keycloak
# exec /opt/keycloak/bin/kc.sh start-dev
# ENTRYPOINT ["/opt/keycloak/bin/kc.sh"]
# CMD ["start-dev", "--optimized"]
/opt/keycloak/bin/kcadm.sh config credentials --server http://localhost:8080/health/ready --realm master --user admin --password admin
/opt/keycloak/bin/kcadm.sh get http://localhost:9000/${KC_HTTP_RELATIVE_PATH}/health